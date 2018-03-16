using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Represents whole build session of the one Unit, all dependency of the built unit are built in context of one build session
  /// </summary>
  public class BuildSession
  {
    private readonly BuildPlansCollection _buildPlans;
    private readonly List<UnitInfo> _buildSequence;
    private readonly IEnumerable<object> _buildStages;
    [CanBeNull]
    private readonly BuildPlansCollection _auxBuildPlans;

    private BuildSession(
      [NotNull] IEnumerable<object> buildStages,
      [NotNull] BuildPlansCollection buildPlans,
      [CanBeNull] BuildPlansCollection auxBuildPlans)
    {
      if (buildStages == null) throw new ArgumentNullException(nameof(buildStages));
      if (buildPlans == null) throw new ArgumentNullException(nameof(buildPlans));

      _buildStages = buildStages;
      _buildPlans = buildPlans;
      _auxBuildPlans = auxBuildPlans;
      _buildSequence = new List<UnitInfo>(4);
    }
   
    /// <summary>
    ///   Builds a Unit represented by <paramref name="unitInfo" />
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher" /> for details</param>
    /// <param name="buildStages">The conveyer of build stages. <see cref="Builder" /> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit" /> method </param>
    public static BuildResult BuildUnit(
      [NotNull] UnitInfo unitInfo,
      [NotNull] IEnumerable<object> buildStages,
      [NotNull] BuildPlansCollection buildPlans,
      [CanBeNull] BuildPlansCollection runtimeBuildPlans) => new BuildSession(buildStages, buildPlans, runtimeBuildPlans).BuildUnit(unitInfo);

    [CanBeNull][Pure]
    public BuildResult BuildUnit([NotNull] UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      using (LogBuildSessionState(unitInfo))
      {
        _buildSequence.Add(unitInfo);
        try
        {
          MatchedBuildActions actions;
          MatchedBuildActions auxActions;
          using(Log.Block(LogLevel.Verbose, "Looking for build actions"))
          {
            actions = _buildPlans.GetBuildActions(_buildSequence);
            auxActions = _auxBuildPlans?.GetBuildActions(_buildSequence);
          }
          Log.Verbose("");
          return BuildUnit(actions.Merge(auxActions));
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
      if (matchedBuildActions == null) return null;
      
      var performedActions = new Stack<IBuildAction>();

      // builder to pass into IBuldActon.Execute
      var unitBuilder = new UnitBuilder(_buildSequence, this);
      
      foreach (var stage in _buildStages)
      {
        var buildAction = matchedBuildActions.GetTopmostAction(stage);
        if (buildAction == null)
          continue;

        performedActions.Push(buildAction);

        using(Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Execute action", buildAction))
          buildAction.Process(unitBuilder);

        if (unitBuilder.BuildResult != null)
        {
          Log.Info("");
          Log.Info("Built unit{{{0}}}", unitBuilder.BuildResult);
          break; // object is built, unwind called actions in reverse orders
        }
      }

      foreach (var buildAction in performedActions)
      {
        Log.Info("");
        using(Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Rewind action", buildAction))
          buildAction.PostProcess(unitBuilder);
      }

      return unitBuilder.BuildResult;
    }

    private IDisposable LogBuildSessionState(UnitInfo unitInfo)
    {
      var block = Log.Block(LogLevel.Info, LogConst.OneParameterFormat, "Build", unitInfo);
      {
        _buildSequence.LogBuildSequence();
      }
      return block;
    }
  }
}