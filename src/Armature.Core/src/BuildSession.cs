using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
///   Represents whole build session of the one Unit, all dependency of the built unit are built in context of one build session.
/// </summary>
/// <remarks>It could be for example IA -> A -> IB -> B -> int. This chain means that for now unit of type int is under construction
/// but it is built in the "context" of the whole build chain.</remarks>
public partial class BuildSession
{
  private const string GatherBuildActions = "GatherBuildActions";
  private const string ParentBuilder      = "ParentBuilder";

  private readonly object[]          _buildStages;
  private readonly IBuildChainPattern  _mainBuildChainPatternTree;
  private readonly IBuildChainPattern? _auxPatternTree;
  private readonly Builder[]?        _parentBuilders;
  private readonly List<UnitId>      _buildChain;

  /// <param name="buildStages">The sequence of build stages. See <see cref="Builder" /> for details.</param>
  /// <param name="buildPlans">Build plans used to find build actions to build a unit.</param>
  /// <param name="auxPatternTree">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method
  /// they are passed to <paramref name="parentBuilders"/> in opposite to <paramref name="buildPlans"/> </param>
  /// <param name="parentBuilders">
  ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
  ///   parent builders one by one in the order they passed into constructor
  /// </param>
  public BuildSession(object[] buildStages, IBuildChainPattern buildPlans, IBuildChainPattern? auxPatternTree, Builder[]? parentBuilders)
  {
    _buildStages     = buildStages ?? throw new ArgumentNullException(nameof(buildStages));
    _mainBuildChainPatternTree = buildPlans  ?? throw new ArgumentNullException(nameof(buildPlans));
    _auxPatternTree  = auxPatternTree;
    _parentBuilders  = parentBuilders;
    _buildChain   = new List<UnitId>(4);
  }

  /// <summary>
  ///   Builds a Unit represented by <paramref name="unitId" />
  /// </summary>
  /// <param name="unitId">"Id" of the unit to build. See <see cref="IBuildChainPattern" /> for details</param>
  public BuildResult BuildUnit(UnitId unitId) => Build(unitId, BuildUnit);

  /// <summary>
  ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  ///   This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">"Id" of the unit to build. See <see cref="IBuildChainPattern" /> for details</param>
  public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => Build(unitId, BuildAllUnits);

  /// <summary>
  /// Common logic to build one or all units
  /// </summary>
  private T Build<T>(UnitId unitId, Func<WeightedBuildActionBag?, T> build)
  {
    T result;
    using(Log.NamedBlock(LogLevel.Info, "Build"))
    {
      Log.WriteLine(LogLevel.Verbose, () => $"UnitId = {unitId.ToHoconString()}");
      _buildChain.Add(unitId);

      WeightedBuildActionBag? actions;
      WeightedBuildActionBag? auxActions;

      using(Log.NamedBlock(LogLevel.Verbose, GatherBuildActions))
      {
        Log.WriteLine(LogLevel.Verbose, () => $"Context = {_buildChain.ToHoconString()}" );

        var buildChain = _buildChain.AsArrayTail();
        actions    = _mainBuildChainPatternTree.GatherBuildActions(buildChain, 0);
        auxActions = _auxPatternTree?.GatherBuildActions(buildChain, 0);
      }

      var actionBag = actions.Merge(auxActions);
      LogGatheredActions(actionBag);

      try {
        result = build(actionBag);
      }
      finally {
        _buildChain.RemoveAt(_buildChain.Count - 1);
      }
    }
    Log.WriteLine(LogLevel.Info, "");
    return result;
  }

  private BuildResult BuildUnit(WeightedBuildActionBag? buildActionBag)
  {
    if(buildActionBag is null)
      return BuildViaParentBuilder(_buildChain.Last());

    // builder to pass into IBuildActon.Execute
    var buildSession     = new Interface(this, _buildChain);
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

    LogBuildResult(buildSession.BuildResult);

    foreach(var buildAction in performedActions)
      BuildActionPostProcess(buildAction, buildSession);

    return buildSession.BuildResult.HasValue
             ? buildSession.BuildResult
             : BuildViaParentBuilder(_buildChain.Last());
  }

  private List<Weighted<BuildResult>> BuildAllUnits(WeightedBuildActionBag? buildActionBag)
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
      var buildSession = new Interface(this, _buildChain);

      var buildAction = weightedBuildAction.Entity;
      BuildActionProcess(buildAction, buildSession);
      BuildActionPostProcess(buildAction, buildSession);

      LogBuildResult(buildSession.BuildResult);

      if(buildSession.BuildResult.HasValue)
        buildResultList.Add(buildSession.BuildResult.WithWeight(weightedBuildAction.Weight));
    }

    return buildResultList;
  }

  private void BuildActionProcess(IBuildAction buildAction, Interface buildSession)
  {
    using(Log.NamedBlock(LogLevel.Verbose, () => LogConst.BuildAction_Process(buildAction)))
      try {
        buildAction.Process(buildSession);
      }
      catch(Exception exc) {
        using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_Process(buildAction)}.Exception: "))
          exc.WriteToLog();
        AddBuildSessionData(exc); // add build session data after logging the exception in order that data don't pollute the log, this data is already there
        throw;
      }
  }

  private void BuildActionPostProcess(IBuildAction buildAction, IBuildSession buildSession)
  {
    using(Log.NamedBlock(LogLevel.Trace, () => LogConst.BuildAction_PostProcess(buildAction)))
    {
      Log.WriteLine(LogLevel.Trace, () => $"Build.Result = {buildSession.BuildResult.ToLogString()}");
      try {
        buildAction.PostProcess(buildSession);
      }
      catch(Exception exc) {
        using(Log.NamedBlock(LogLevel.Info, () => $"{LogConst.BuildAction_PostProcess(buildAction)}.Exception: "))
          exc.WriteToLog();
        AddBuildSessionData(exc); // add build session data after logging the exception in order that data don't pollute the log, this data is already there
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
      catch(Exception exc)
      {
        exceptions.Add(exc); // it's already written to the log by the parent builder, so just collect it
        // continue
      }

    return exceptions.Count > 0
             ? throw new AggregateException(
                   $"{exceptions.Count} exceptions occured during during building an unit via parent builders."
                 + $"See {nameof(Exception)}.{nameof(Exception.Data)} and {nameof(AggregateException)}.{nameof(AggregateException.InnerExceptions)}"
                 + $" for details or enable logging using {nameof(Log)}.{nameof(Log.Enable)} to investigate the error.",
                   exceptions)
                .AddData(ExceptionConst.Logged, true)
             : default;
  }

  private void AddBuildSessionData(Exception exception)
  {
    if(exception.Data.Contains(ExceptionConst.Context)) return;
    exception.AddData(ExceptionConst.Context, _buildChain);
  }

  private static void LogBuildResult(BuildResult buildResult)
  {
    Log.WriteLine(LogLevel.Info, "");
    Log.WriteLine(LogLevel.Info, () => $"Build.Result = {buildResult.ToLogString()}");
    Log.WriteLine(LogLevel.Info, "");
  }

  private static void LogGatheredActions(WeightedBuildActionBag? actionBag)
  {
    Log.WriteLine(LogLevel.Info, "");
    Log.Write(LogLevel.Info, $"{GatherBuildActions}.Result: ");
    actionBag.WriteToLog(LogLevel.Info);
    Log.WriteLine(LogLevel.Info, "");
  }
}