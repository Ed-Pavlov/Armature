using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Represents whole build session of one Unit, all dependency of the being built unit are built in the context of one build session.
/// </summary>
/// <remarks>It could be for example IA -> A -> IB -> B -> int. This stack means that for now Unit of type int is the target unit
/// but it is built in the "context" of the whole build stack.</remarks>
public partial class BuildSession
{
  private const string GatherBuildActions = "GatherBuildActions";
  private const string ParentBuilder      = "ParentBuilder";

  private readonly object[]            _buildStages;
  private readonly IBuildStackPattern  _mainBuildStackPatternTree;
  private readonly IBuildStackPattern? _auxPatternTree;
  private readonly IBuilder[]?         _parentBuilders;
  private readonly List<UnitId>        _buildStackList = new List<UnitId>(4);

  /// <param name="buildStages">The sequence of build stages. See <see cref="Builder" /> for details.</param>
  /// <param name="patternTree">Build stack patterns tree used to find build actions to build a unit.</param>
  /// <param name="auxPatternTree">Additional build stack patterns tree, in opposite to <paramref name="patternTree"/> these patterns
  /// are passed to <paramref name="parentBuilders"/> if unit is being tried to build via parent builders.</param>
  /// <param name="parentBuilders">
  /// If unit is not built and <paramref name="parentBuilders" /> are provided, tries to build a unit using
  /// parent builders one by one in the order they passed into the constructor.
  /// </param>
  public BuildSession(object[] buildStages, IBuildStackPattern patternTree, IBuildStackPattern? auxPatternTree, IBuilder[]? parentBuilders)
  {
    _buildStages = buildStages ?? throw new ArgumentNullException(nameof(buildStages));
    if(buildStages.Length == 0) throw new ArgumentException("Should contain at least one build stage", nameof(buildStages));
    if(buildStages.Any(stage => stage is null)) throw new ArgumentException("Should not contain null values", nameof(buildStages));
    if(buildStages.Length != buildStages.Distinct().Count()) throw new ArgumentException("Should not contain duplicate values", nameof(buildStages));
    if(parentBuilders?.Any(_ => _ is null) == true) throw new ArgumentException("Should not contain null values", nameof(parentBuilders));

    _mainBuildStackPatternTree = patternTree ?? throw new ArgumentNullException(nameof(patternTree));
    _auxPatternTree            = auxPatternTree;
    _parentBuilders            = parentBuilders;
  }

  /// <summary>
  /// Builds a Unit represented by <paramref name="unitId" />
  /// </summary>
  /// <param name="unitId">"Id" of the unit to build. See <see cref="IBuildStackPattern" /> for details</param>
  public BuildResult BuildUnit(UnitId unitId)
  {
    using(Log.NamedBlock(LogLevel.Info, "Build", true))
      return Build(unitId, BuildUnit);
  }

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">"Id" of the unit to build. See <see cref="IBuildStackPattern" /> for details</param>
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId)
  {
    using(Log.NamedBlock(LogLevel.Info, "BuildAll", true))
      return Build(unitId, BuildAllUnits);
  }

  /// <summary>
  /// Common logic to build one or all units
  /// </summary>
  private T Build<T>(UnitId unitId, Func<BuildSession.Stack, WeightedBuildActionBag?, T> build)
  {
    Log.WriteLine(LogLevel.Info, () => $"Time: \"{DateTime.Now:yyyy-mm-dd HH:mm:ss.fff}\"");
    Log.WriteLine(LogLevel.Info, () => $"Thread: {Environment.CurrentManagedThreadId.ToHoconString()} ");

    T result;

    _buildStackList.Add(unitId);
    var stack = new BuildSession.Stack(_buildStackList);

    Log.WriteLine(LogLevel.Info, () => $"BuildStack = {stack.ToHoconString()}");

    try
    {
      WeightedBuildActionBag? actions;
      WeightedBuildActionBag? auxActions = null;

      Log.WriteLine(LogLevel.Verbose, "");

      using(Log.NamedBlock(LogLevel.Verbose, GatherBuildActions))
      {
        _mainBuildStackPatternTree.GatherBuildActions(stack, out actions);
        _auxPatternTree?.GatherBuildActions(stack, out auxActions);
      }

      var actionBag = actions.Merge(auxActions);
      Log_GatheredActions(actionBag);

      result = build(stack, actionBag);
    }
    catch(Exception exception)
    {
      if(!exception.Data.Contains(ExceptionConst.BuildStack))
        exception.AddData(ExceptionConst.BuildStack, stack.ToHoconString());

      throw;
    }
    finally
    {
      _buildStackList.RemoveAt(_buildStackList.Count - 1);
    }

    return result;
  }

  private BuildResult BuildUnit(BuildSession.Stack stack, WeightedBuildActionBag? buildActionBag)
  {
    if(buildActionBag is null)
      return BuildViaParentBuilder(stack.TargetUnit);

    // builder to pass into IBuildActon.Execute
    var buildSession     = new Interface(this, stack);
    var performedActions = new Stack<IBuildAction>();

    foreach(var stage in _buildStages)
    {
      var buildAction = buildActionBag.GetTopmostAction(stage);

      if(buildAction is null)
        continue;

      performedActions.Push(buildAction);

      BuildActionProcess(buildAction, buildSession);

      if(buildSession.BuildResult.HasValue)
        break; // object is built, unwind called actions in reverse orders
    }

    Log_BuildResult(buildSession.BuildResult);

    foreach(var buildAction in performedActions)
      BuildActionPostProcess(buildAction, buildSession);

    return buildSession.BuildResult.HasValue
               ? buildSession.BuildResult
               : BuildViaParentBuilder(stack.TargetUnit);
  }

  private List<Weighted<BuildResult>> BuildAllUnits(BuildSession.Stack stack, WeightedBuildActionBag? buildActionBag)
  {
    if(buildActionBag is null) return Empty<Weighted<BuildResult>>.List;

    if(buildActionBag.Keys.Count > 1)
    {
      var exception = new ArmatureException($"Actions only for one stage should be provided for {nameof(BuildAllUnits)}");

      var number = 1;

      foreach(var pair in buildActionBag)
        exception.AddData($"Stage #{number++}", pair.Key);

      throw exception;
    }

    var buildResultList = new List<Weighted<BuildResult>>();

    foreach(var weightedBuildAction in buildActionBag.Values.Single())
    {
      var buildSession = new Interface(this, stack);

      var buildAction = weightedBuildAction.Entity;
      BuildActionProcess(buildAction, buildSession);
      Log_BuildActionResult(buildAction, buildSession.BuildResult);
      BuildActionPostProcess(buildAction, buildSession);
      Log.WriteLine(LogLevel.Info, "");

      if(buildSession.BuildResult.HasValue)
        buildResultList.Add(buildSession.BuildResult.WithWeight(weightedBuildAction.Weight));
    }

    Log_BuildAllResult(buildResultList);
    return buildResultList;
  }

  private static void BuildActionProcess(IBuildAction buildAction, IBuildSession buildSession)
  {
    using(Log.NamedBlock(LogLevel.Info, () => LogConst.BuildAction_Process(buildAction)))
      try
      {
        buildAction.Process(buildSession);
      }
      catch(Exception exception)
      {
        if(!exception.Data.Contains(ExceptionConst.Logged))
          using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_Process(buildAction)}.Exception: "))
            exception.WriteToLog();

        throw;
      }
  }

  private static void BuildActionPostProcess(IBuildAction buildAction, IBuildSession buildSession)
  {
    using(Log.NamedBlock(LogLevel.Info, () => LogConst.BuildAction_PostProcess(buildAction)))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"Build.Result = {buildSession.BuildResult.ToLogString()}");

      try
      {
        buildAction.PostProcess(buildSession);
      }
      catch(Exception exc)
      {
        using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_PostProcess(buildAction)}.Exception: "))
          exc.WriteToLog();

        throw;
      }
    }
  }

  private BuildResult BuildViaParentBuilder(UnitId unitId)
  {
    if(_parentBuilders is null) return default;

    var exceptions = new List<Exception>();

    for(var i = 0; i < _parentBuilders.Length; i++)
      try
      {
        var parentBuilderNumber = i + 1;

        using(Log.NamedBlock(LogLevel.Info, () => $"{ParentBuilder} #{parentBuilderNumber}".Quote()))
        {
          var buildResult = _parentBuilders[i].BuildUnit(unitId, _auxPatternTree);

          if(buildResult.HasValue)
            return buildResult;
        }
      }
      catch(ArmatureException exc)
      {
        exceptions.Add(exc); // it's already written to the log by the parent builder, so just collect it

        // continue
      }

    if(exceptions.Count == 1)
      ExceptionDispatchInfo.Capture(exceptions[0]).Throw();

    if(exceptions.Count > 0)
      throw new ArmatureException(
              $"{exceptions.Count} exceptions occured during during building an unit via parent builders."
            + LogConst.ArmatureExceptionPostfix($" and {nameof(ArmatureException)}.{nameof(ArmatureException.InnerExceptions)}"),
              exceptions)
         .AddData(ExceptionConst.Logged, true);

    return default;
  }

  private static void Log_BuildResult(BuildResult buildResult)
  {
    Log.WriteLine(LogLevel.Info, "");
    Log.WriteLine(LogLevel.Info, () => $"Build.Result = {buildResult.ToLogString()}");
    Log.WriteLine(LogLevel.Info, "");
  }

  private static void Log_BuildAllResult(List<Weighted<BuildResult>> buildResultList)
  {
    Log.WriteLine(LogLevel.Info, () => $"BuildAll.Result = {buildResultList.ToHoconString()}");
    Log.WriteLine(LogLevel.Info, "");
  }

  private static void Log_BuildActionResult(IBuildAction buildAction, BuildResult buildResult)
    => Log.WriteLine(
        buildResult.HasValue ? LogLevel.Info : LogLevel.Trace,
        () => $"{LogConst.BuildAction_Name(buildAction)}.Result = {buildResult.ToLogString()}");

  private static void Log_GatheredActions(WeightedBuildActionBag? actionBag)
  {
    Log.WriteLine(LogLevel.Info, "");
    actionBag.WriteToLog(LogLevel.Info, $"{GatherBuildActions}.Result: ");
    Log.WriteLine(LogLevel.Info, "");
  }
}