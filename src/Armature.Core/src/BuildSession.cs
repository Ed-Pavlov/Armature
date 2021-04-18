using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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
    private readonly object[]              _buildStages;
    private readonly IPatternTreeNode  _buildPlans;
    private readonly IPatternTreeNode? _auxBuildPlans;
    private readonly Builder[]?            _parentBuilders;
    private readonly List<UnitId>          _buildSequence;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
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
    /// <param name="unitId">"Id" of the unit to build.</param>
    /// <param name="buildStages">The sequence of build stages. See <see cref="Builder" /> for details.</param>
    /// <param name="buildPlans">Build plans used to find build actions to build a unit.</param>
    /// <param name="auxBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method
    /// they are passed to <paramref name="parentBuilders"/> in opposite to <paramref name="buildPlans"/> </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static BuildResult BuildUnit(
      UnitId                unitId,
      object[]              buildStages,
      IPatternTreeNode  buildPlans,
      IPatternTreeNode? auxBuildPlans,
      Builder[]?            parentBuilders)
      => new BuildSession(buildStages, buildPlans, auxBuildPlans, parentBuilders).BuildUnit(unitId);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    /// <param name="buildStages">The sequence of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to find build actions to build a unit.</param>
    /// <param name="auxBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method
    /// they are passed to <paramref name="parentBuilders"/> in opposite to <paramref name="buildPlans"/> </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static IReadOnlyList<BuildResult> BuildAllUnits(
      UnitId                unitId,
      object[]              buildStages,
      IPatternTreeNode  buildPlans,
      IPatternTreeNode? auxBuildPlans,
      Builder[]?            parentBuilders)
      => new BuildSession(buildStages, buildPlans, auxBuildPlans, parentBuilders).BuildAllUnits(unitId);

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitId" />
    /// </summary>
    /// <param name="unitId">"Id" of the unit to build. See <see cref="IPatternTreeNode" /> for details</param>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public BuildResult BuildUnit(UnitId unitId) => Build(unitId, BuildUnit);

    /// <summary>
    ///   Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
    ///   This can be useful to build all implementers of an interface.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public IReadOnlyList<BuildResult> BuildAllUnits(UnitId unitId) => Build(unitId, BuildAllUnits);

    /// <summary>
    /// Common logic to build one or all units
    /// </summary>
    private T Build<T>(UnitId unitId, Func<BuildActionBag?, T> build)
    {
      using(LogBuildSessionState(unitId))
      {
        _buildSequence.Add(unitId);

        try
        {
          BuildActionBag? actions;
          BuildActionBag? auxActions;

          using(Log.Block(LogLevel.Verbose, "Looking for build actions"))
          {
            actions    = _buildPlans.GatherBuildActions(_buildSequence.AsArrayTail(), 0);
            auxActions = _auxBuildPlans?.GatherBuildActions(_buildSequence.AsArrayTail(), 0);
          }

          Log.WriteLine(LogLevel.Verbose, "");

          return build(actions.Merge(auxActions));
        }
        catch(Exception exception)
        {
          AddDebugData(exception);
          throw;
        }
        finally
        {
          _buildSequence.RemoveAt(_buildSequence.Count - 1);
        }
      }
    }

    private BuildResult BuildUnit(BuildActionBag? buildActionBag)
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

        using(Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Execute action", buildAction)))
        {
          buildAction.Process(buildSession);
        }

        if(buildSession.BuildResult.HasValue)
        {
          Log.WriteLine(LogLevel.Info, "");

          Log.WriteLine(
            LogLevel.Info,
            () => string.Format("Build Result{{{0}:{1}}}", buildSession.BuildResult, buildSession.BuildResult.Value?.GetType().ToLogString()));

          break; // object is built, unwind called actions in reverse orders
        }
      }

      foreach(var buildAction in performedActions)
      {
        Log.WriteLine(LogLevel.Info, "");

        using(Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Rewind action", buildAction)))
        {
          buildAction.PostProcess(buildSession);
        }
      }

      return buildSession.BuildResult.HasValue
               ? buildSession.BuildResult
               : BuildViaParentBuilder(_buildSequence.Last());
    }

    private List<BuildResult> BuildAllUnits(BuildActionBag? buildActionBag)
    {
      if(buildActionBag is null) return Empty<BuildResult>.List;

      if(buildActionBag.Keys.Count > 1)
        throw new ArmatureException($"Actions only for one stage should be provided for {nameof(BuildAllUnits)}");

      var result = new List<BuildResult>();

      foreach(var buildAction in buildActionBag.Values.Single().Select(_ => _.Entity))
      {
        var buildSession = new Interface(this, _buildSequence);

        using(Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Execute action", buildAction)))
        {
          buildAction.Process(buildSession);
          buildAction.PostProcess(buildSession);
        }

        if(buildSession.BuildResult.HasValue)
        {
          Log.WriteLine(LogLevel.Info, "");

          Log.WriteLine(
            LogLevel.Info,
            () => string.Format("Build Result{{{0}:{1}}}", buildSession.BuildResult, buildSession.BuildResult.Value?.GetType().ToLogString()));

          result.Add(buildSession.BuildResult);
        }
      }

      return result;
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
          exceptions.Add(exc);

          // continue
        }

      if(exceptions.Count == 0)
        return default;

      throw exceptions.Aggregate("One or more exceptions occured during build the unit");
    }

    private IDisposable LogBuildSessionState(UnitId unitId)
    {
      var block = Log.Block(LogLevel.Info, () => string.Format(LogConst.OneParameterFormat, "Build", unitId));

      {
        _buildSequence.ToLog();
      }

      return block;
    }

    private void AddDebugData(Exception exception)
    {
      if(exception.Data.Contains(ExceptionData.BuildSequence)) return;

      var sb = new StringBuilder();

      foreach(var unitInfo in _buildSequence)
        sb.AppendLine(unitInfo.ToString());

      exception.AddData(ExceptionData.BuildSequence, sb.ToString());
    }
  }
}
