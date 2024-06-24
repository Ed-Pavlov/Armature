using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature.Core;

/// <summary>
/// Represents whole build session of one Unit, all dependency of the being built Unit are built in the context of one build session.
/// </summary>
/// <remarks>It could be for example IA -> A -> IB -> B -> int. This stack means that for now Unit of type int is the target unit,
/// but it is built in the "context" of the whole build stack.</remarks>
public partial class BuildSession
{
  private const string GatherBuildActions = "GatherBuildActions";

  private readonly object[]            _buildStages;
  private readonly IBuildStackPattern  _mainBuildStackPatternTree;
  private readonly IBuildStackPattern? _auxPatternTree;
  private readonly IBuilder[]          _parentBuilders;
  private readonly List<UnitId>        _buildStackList = new(4);

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
    _parentBuilders            = parentBuilders ?? Empty<IBuilder>.Array;
  }

  /// <inheritdoc cref="IBuildSession.BuildUnit"/>
  public BuildResult BuildUnit(UnitId unitId, bool engageParentBuilders = true)
  {
    using(Log.NamedBlock(LogLevel.Info, nameof(BuildUnit), true))
      return Build(unitId, (stack, bag) => BuildUnit(stack, bag, engageParentBuilders));
  }

  /// <inheritdoc cref="IBuildSession.BuildAllUnits"/>
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, bool engageParentBuilders = true)
  {
    using(Log.NamedBlock(LogLevel.Info, nameof(BuildAllUnits), true))
      return Build(unitId, (stack, bag) => BuildAllUnits(stack, bag, engageParentBuilders));
  }

  /// <summary>
  /// Common logic to build one or all units
  /// </summary>
  private T Build<T>(UnitId unitId, Func<Stack, WeightedBuildActionBag?, T> build)
  {
    if(Log.IsEnabled())
    {
      Log.WriteLine(LogLevel.Info, $"Time: \"{DateTime.Now:yyyy-mm-dd HH:mm:ss.fff}\"");
      Log.WriteLine(LogLevel.Info, $"Thread: {Environment.CurrentManagedThreadId.ToHoconString()} ");
    }

    T result;

    _buildStackList.Add(unitId);
    var stack = new Stack(_buildStackList);

    if(Log.IsEnabled())
      Log.WriteLine(LogLevel.Info, $"BuildStack = {stack.ToHoconString()}");

    try
    {
      WeightedBuildActionBag? actions;
      WeightedBuildActionBag? auxActions = null;

      Log.WriteLine(LogLevel.Verbose, "");

      using(Log.NamedBlock(LogLevel.Verbose, nameof(GatherBuildActions)))
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

  private BuildResult BuildUnit(Stack stack, WeightedBuildActionBag? buildActionBag, bool engageParentBuilders)
  {
    if(buildActionBag is null)
      return engageParentBuilders ? BuildViaParentBuilder(stack.TargetUnit) : default;

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

  private List<Weighted<BuildResult>> BuildAllUnits(Stack stack, WeightedBuildActionBag? buildActionBag, bool engageParentBuilders)
  {
    if(buildActionBag is null)
      return engageParentBuilders ? BuildAllViaParentBuilder(stack.TargetUnit) : Empty<Weighted<BuildResult>>.List;

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

    Log_BuildAllResult("BuildAll.Result", buildResultList);

    if(engageParentBuilders)
      BuildAllViaParentBuilder(stack.TargetUnit, buildResultList);

    return buildResultList;
  }

  private static void BuildActionProcess(IBuildAction buildAction, IBuildSession buildSession)
  {
    // ReSharper disable once ConvertClosureToMethodGroup - method group is worse in terms of performance
    using(Log.NamedBlock(LogLevel.Info, () => buildAction.ProcessMethod()))
      try
      {
        buildAction.Process(buildSession);
      }
      catch(Exception exception)
      {
        if(!exception.Data.Contains(ExceptionConst.Logged))
          using(Log.NamedBlock(LogLevel.Info, () => $"{buildAction.ProcessMethod()}.Exception: "))
            exception.WriteToLog();

        throw;
      }
  }

  private static void BuildActionPostProcess(IBuildAction buildAction, IBuildSession buildSession)
  {
    using(Log.NamedBlock(LogLevel.Info, () => buildAction.PostProcessMethod()))
    {
      if(Log.IsEnabled(LogLevel.Verbose))
        Log.WriteLine(LogLevel.Verbose, $"Build.Result = {buildSession.BuildResult.ToLogString()}");

      try
      {
        buildAction.PostProcess(buildSession);
      }
      catch(Exception exc)
      {
        using(Log.NamedBlock(LogLevel.Info, () => $"{buildAction.PostProcessMethod()}.Exception: "))
          exc.WriteToLog();

        throw;
      }
    }
  }

  private List<Weighted<BuildResult>> BuildAllViaParentBuilder(UnitId unitId, List<Weighted<BuildResult>>? buildResultList = null)
  {
    buildResultList ??= [];
    foreach(var parentBuilder in _parentBuilders)
    {
      var list = parentBuilder.BuildAllUnits(unitId);
      buildResultList.AddRange(list);
    }

    return buildResultList;
  }

  private BuildResult BuildViaParentBuilder(UnitId unitId)
  {
    var exceptions = new List<Exception>();

    foreach(var parentBuilder in _parentBuilders)
      try
      {
        using(Log.NamedBlock(LogLevel.Info, () => $"{parentBuilder.Name}"))
        {
          var buildResult = parentBuilder.BuildUnit(unitId, _auxPatternTree);

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
          $"{exceptions.Count} exceptions occured during during building a unit via parent builders."
        + LogConst.ArmatureExceptionPostfix($" and {nameof(ArmatureException)}.{nameof(ArmatureException.InnerExceptions)}"),
          exceptions)
       .AddData(ExceptionConst.Logged, true);

    return default;
  }

  private static void Log_BuildResult(BuildResult buildResult)
  {
    if(Log.IsEnabled())
    {
      Log.WriteLine(LogLevel.Info, "");
      Log.WriteLine(LogLevel.Info, $"Build.Result = {buildResult.ToLogString()}");
      Log.WriteLine(LogLevel.Info, "");
    }
  }

  private static void Log_BuildAllResult(string title, List<Weighted<BuildResult>> buildResultList)
  {
    if(Log.IsEnabled())
    {
      Log.WriteLine(LogLevel.Info, $"{title} = {buildResultList.ToHoconString()}");
      Log.WriteLine(LogLevel.Info, "");
    }
  }

  private static void Log_BuildActionResult(IBuildAction buildAction, BuildResult buildResult)
  {
    var logLevel = buildResult.HasValue ? LogLevel.Info : LogLevel.Trace;

    if(Log.IsEnabled(logLevel))
      Log.WriteLine(logLevel, $"{buildAction.GetName()}.Result = {buildResult.ToLogString()}");
  }

  private static void Log_GatheredActions(WeightedBuildActionBag? actionBag)
  {
    Log.WriteLine(LogLevel.Info, "");
    actionBag.WriteToLog(LogLevel.Info, $"{nameof(GatherBuildActions)}.Result: ");
    Log.WriteLine(LogLevel.Info, "");
  }
}
