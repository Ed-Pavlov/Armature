using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature.Core.Common;
using Armature.Core.Logging;
using Resharper.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Represents whole build session of the one Unit, all dependency of the built unit are built in context of one build session
  /// </summary>
  public partial class BuildSession
  {
    [CanBeNull]
    private readonly Builder[] _parentBuilders;
    [CanBeNull]
    private readonly BuildPlansCollection _auxBuildPlans;
    private readonly BuildPlansCollection _buildPlans;
    private readonly List<UnitInfo> _buildSequence;
    private readonly IEnumerable<object> _buildStages;

    private BuildSession(
      [NotNull] IEnumerable<object> buildStages,
      [NotNull] BuildPlansCollection buildPlans,
      [CanBeNull] BuildPlansCollection auxBuildPlans,
      [CanBeNull] Builder[] parentBuilders)
    {
      if (buildStages == null) throw new ArgumentNullException(nameof(buildStages));
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      _buildStages = buildStages;
      _buildPlans = buildPlans;
      _auxBuildPlans = auxBuildPlans;
      _buildSequence = new List<UnitInfo>(4);
      _parentBuilders = parentBuilders;
    }

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    /// <param name="buildStages">The conveyer of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static BuildResult BuildUnit(
      [NotNull] UnitInfo unitInfo,
      [NotNull] IEnumerable<object> buildStages,
      [NotNull] BuildPlansCollection buildPlans,
      [CanBeNull] BuildPlansCollection runtimeBuildPlans,
      [CanBeNull] Builder[] parentBuilders) => new BuildSession(buildStages, buildPlans, runtimeBuildPlans, parentBuilders).BuildUnit(unitInfo);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    /// <param name="buildStages">The conveyer of build stages. See <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    /// <param name="parentBuilders">
    ///   If unit is not built and <paramref name="parentBuilders" /> are provided, trying to build a unit using
    ///   parent builders one by one in the order they passed into constructor
    /// </param>
    public static IReadOnlyList<BuildResult> BuildAllUnits(
      [NotNull] UnitInfo unitInfo,
      [NotNull] IEnumerable<object> buildStages,
      [NotNull] BuildPlansCollection buildPlans,
      [CanBeNull] BuildPlansCollection runtimeBuildPlans,
      [CanBeNull] Builder[] parentBuilders) => new BuildSession(buildStages, buildPlans, runtimeBuildPlans, parentBuilders).BuildAllUnits(unitInfo);

    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    public BuildResult BuildUnit(UnitInfo unitInfo) => Build(unitInfo, BuildUnit);

    /// <summary>
    ///   Builds all Units represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    public IReadOnlyList<BuildResult> BuildAllUnits(UnitInfo unitInfo) => Build(unitInfo, BuildAllUnits);

    [CanBeNull]
    [Pure]
    private T Build<T>([NotNull] UnitInfo unitInfo, Func<MatchedBuildActions, T> build)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      using (LogBuildSessionState(unitInfo))
      {
        _buildSequence.Add(unitInfo);
        try
        {
          MatchedBuildActions actions;
          MatchedBuildActions auxActions;
          using (Log.Block(LogLevel.Verbose, "Looking for build actions"))
          {
            actions = _buildPlans.GetBuildActions(_buildSequence.AsArrayTail());
            auxActions = _auxBuildPlans?.GetBuildActions(_buildSequence.AsArrayTail());
          }

          Log.WriteLine(LogLevel.Verbose, "");
          return build(actions.Merge(auxActions));
        }
        finally
        {
          _buildSequence.RemoveAt(_buildSequence.Count - 1);
        }
      }
    }

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    private BuildResult BuildUnit(MatchedBuildActions matchedBuildActions)
    {
      if (matchedBuildActions == null)
        return BuildViaParentBuilder();

      // builder to pass into IBuldActon.Execute
      var unitBuilder = new Interface(_buildSequence, this);
      var performedActions = new Stack<IBuildAction>();

      foreach (var stage in _buildStages)
      {
        var buildAction = matchedBuildActions.GetTopmostAction(stage);
        if (buildAction == null)
          continue;

        performedActions.Push(buildAction);

        using (Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Execute action", buildAction))
        {
          buildAction.Process(unitBuilder);
        }

        if (unitBuilder.BuildResult != null)
        {
          Log.WriteLine(LogLevel.Info, "");
          Log.WriteLine(LogLevel.Info, "Build Result{{{0}}}", unitBuilder.BuildResult);
          break; // object is built, unwind called actions in reverse orders
        }
      }

      foreach (var buildAction in performedActions)
      {
        Log.WriteLine(LogLevel.Info, "");
        using (Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Rewind action", buildAction))
        {
          buildAction.PostProcess(unitBuilder);
        }
      }
      
      return unitBuilder.BuildResult;
    }

    private BuildResult BuildViaParentBuilder()
    {
      if (_parentBuilders == null) return null;

      for (var i = 0; i < _parentBuilders.Length; i++)
        try
        {
          using (Log.Block(LogLevel.Info, "Try build via parent builder #{0}", i))
          {
            var buildResult = _parentBuilders[i].BuildUnit(_buildSequence.Last(), _auxBuildPlans);
            if (buildResult != null)
              return buildResult;
          }
        }
        catch (Exception)
        {
          // continue;
        }

      return null;
    }

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    private List<BuildResult> BuildAllUnits(MatchedBuildActions matchedBuildActions)
    {
      if (matchedBuildActions == null) return null;

      if (matchedBuildActions.Keys.Count > 1)
        throw new ArmatureException("Actions only for one stage should be provided for BuildAll");

      var result = new List<BuildResult>();
      foreach (var buildAction in matchedBuildActions.Values.Single().Select(_ => _.Entity))
      {
        var unitBuilder = new Interface(_buildSequence, this);

        using (Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Execute action", buildAction))
        {
          buildAction.Process(unitBuilder);
          buildAction.PostProcess(unitBuilder);
        }

        if (unitBuilder.BuildResult != null)
        {
          Log.WriteLine(LogLevel.Info, "");
          Log.WriteLine(LogLevel.Info, "Build Result{{{0}}}", unitBuilder.BuildResult);
          result.Add(unitBuilder.BuildResult);
        }
      }

      return result;
    }

    private IDisposable LogBuildSessionState(UnitInfo unitInfo)
    {
      var block = Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Build", unitInfo);
      {
        _buildSequence.ToLog();
      }
      return block;
    }
  }
}