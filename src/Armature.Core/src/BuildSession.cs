using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Represents whole build session of the one Unit, all dependency of the built unit are built in context of one build session.
  /// </summary>
  /// <remarks>It could be for example IA -> A -> IB -> B -> int. This sequence means that for now unit of type int is under construction
  /// but it is built in "context" of all the sequence of dependencies.</remarks>
  public partial class BuildSession
  {
    private readonly object[]          _buildStages;
    private readonly IPatternTreeNode  _buildPlans;
    private readonly IPatternTreeNode? _auxBuildPlans;
    private readonly Builder[]?        _parentBuilders;
    private readonly List<UnitId>      _buildSequence;

    /// <param name="buildStages">The sequence of build stages. See <see cref="Builder" /> for details.</param>
    /// <param name="buildPlans">Build plans used to find build actions to build a unit.</param>
    /// <param name="auxBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method
    /// they are passed to <paramref name="parentBuilders"/> in opposite to <paramref name="buildPlans"/> </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public BuildSession(object[] buildStages, IPatternTreeNode buildPlans, IPatternTreeNode? auxBuildPlans, Builder[]? parentBuilders)
    {
      _buildStages    = buildStages ?? throw new ArgumentNullException(nameof(buildStages));
      _buildPlans     = buildPlans  ?? throw new ArgumentNullException(nameof(buildPlans));
      _auxBuildPlans  = auxBuildPlans;
      _parentBuilders = parentBuilders;
      _buildSequence  = new List<UnitId>(4);
    }

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    public BuildResult BuildUnit(UnitId unitId) => Build(unitId, BuildUnit);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
    ///   This can be useful to build all implementers of an interface.
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => Build(unitId, BuildAllUnits);

    /// <summary>
    /// Common logic to build one or all units
    /// </summary>
    private T Build<T>(UnitId unitId, Func<WeightedBuildActionBag?, T> build)
    {
      using(Log.Block(LogLevel.Info, () => $"Build( {unitId} )"))
      {
        _buildSequence.Add(unitId);

        WeightedBuildActionBag? actions;
        WeightedBuildActionBag? auxActions;

        using(Log.Block(LogLevel.Verbose, () => $"{nameof(IPatternTreeNode.GatherBuildActions)}( {string.Join(", ", _buildSequence)} )"))
        {
          actions    = _buildPlans.GatherBuildActions(_buildSequence.AsArrayTail(), 0);
          auxActions = _auxBuildPlans?.GatherBuildActions(_buildSequence.AsArrayTail(), 0);
        }

        var actionBag = actions.Merge(auxActions);
        LogGatheredActions(actionBag);
        var result = build(actionBag);
        
        _buildSequence.RemoveAt(_buildSequence.Count - 1);
        return result;
      }
    }

    private BuildResult BuildUnit(WeightedBuildActionBag? buildActionBag)
    {
      if(buildActionBag is null)
        return BuildViaParentBuilder(_buildSequence.Last());

      // builder to pass into IBuildActon.Execute
      var buildSession     = new Interface(this, _buildSequence);
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

      Log.WriteLine(LogLevel.Verbose, "");

      LogBuildResult(buildSession.BuildResult);

      Log.WriteLine(LogLevel.Trace, "");

      foreach(var buildAction in performedActions)
        BuildActionPostProcess(buildAction, buildSession);

      return buildSession.BuildResult.HasValue
               ? buildSession.BuildResult
               : BuildViaParentBuilder(_buildSequence.Last());
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
        var buildSession = new Interface(this, _buildSequence);

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
      Log.WriteLine(LogLevel.Verbose, "");

      using(Log.Block(LogLevel.Verbose, () => $"{buildAction}.{nameof(IBuildAction.Process)}( buildResult: {buildSession.BuildResult} )"))
      {
        try
        {
          buildAction.Process(buildSession);
        }
        catch(Exception exc)
        {
          AddBuildSessionData(exc);
          exc.ToLog(() => $"Exception was thrown during executing {buildAction}.{nameof(IBuildAction.Process)} method");
          throw;
        }
      }
    }

    private void BuildActionPostProcess(IBuildAction buildAction, IBuildSession buildSession)
    {
      using(Log.Block(LogLevel.Trace, () => $"{buildAction}.{nameof(IBuildAction.PostProcess)}( buildResult: {buildSession.BuildResult} )"))
      {
        try
        {
          buildAction.PostProcess(buildSession);
        }
        catch(Exception exc)
        {
          AddBuildSessionData(exc);
          exc.ToLog(() => $"Exception was thrown during executing {buildAction}.{nameof(IBuildAction.PostProcess)} method");
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
          using(Log.Block(LogLevel.Info, "Try build via parent builder #{0}", i))
          {
            var buildResult = _parentBuilders[i].BuildUnit(unitId, _auxBuildPlans);

            if(buildResult.HasValue)
              return buildResult;
          }
        }
        catch(Exception exc)
        {
          exc.ToLog(() => "Exception");
          exceptions.Add(exc);

          // continue
        }

      if(exceptions.Count == 0)
        return default;

      throw exceptions.Aggregate($"{exceptions.Count} exceptions occured during building an unit via parent builders");
    }

    private void AddBuildSessionData(Exception exception)
    {
      if(exception.Data.Contains(ExceptionConst.BuildSequence)) return;
      exception.AddData(ExceptionConst.BuildSequence, string.Join(" -> ", _buildSequence));
    }

    private static void LogBuildResult(BuildResult buildResult)
      => Log.WriteLine(
        LogLevel.Info,
        () => $"{nameof(BuildSession)}.{nameof(IBuildSession.BuildResult)} = "
            + $"{(buildResult.HasValue ? $"{buildResult} : {buildResult.Value?.GetType().ToLogString()}" : buildResult)}");

    private static void LogGatheredActions(WeightedBuildActionBag? actionBag)
    {
      Log.WriteLine(LogLevel.Verbose, "");

      if(actionBag is null)
        Log.WriteLine(LogLevel.Verbose, "No matched actions");
      else
        using(Log.Block(LogLevel.Verbose, "Gathered actions"))
          actionBag.ToLog(LogLevel.Verbose);
    }
  }
}
