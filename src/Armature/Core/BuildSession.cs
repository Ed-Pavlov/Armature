using System;
using System.Collections.Generic;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents whole build session of the one Unit, all dependency of the built unit are built in context of one build session
  /// </summary>
  public class BuildSession
  {
    private readonly IEnumerable<object> _buildStages;
    private readonly List<UnitInfo> _buildSequence;
    
    private readonly BuildPlansCollection _buildPlans;
    [CanBeNull] 
    private readonly BuildPlansCollection _runtimeBuildPlans;

    /// <summary>
    /// Builds a Unit represented by <paramref name="unitInfo"/>
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. See <see cref="IUnitSequenceMatcher"/> for details</param>
    /// <param name="buildStages">The conveyer of build stages. <see cref="Builder"/> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="runtimeBuildPlans">Build plans collection contains additional build plans passed into <see cref="Builder.BuildUnit"/> method </param>
    public static BuildResult BuildUnit(
      [NotNull] UnitInfo unitInfo,
      [NotNull] IEnumerable<object> buildStages,
      [NotNull] BuildPlansCollection buildPlans,
      [CanBeNull] BuildPlansCollection runtimeBuildPlans)
    {
      return new BuildSession(buildStages, buildPlans, runtimeBuildPlans).BuildUnit(unitInfo);
    }
    
    private BuildSession(
      [NotNull] IEnumerable<object> buildStages, 
      [NotNull] BuildPlansCollection buildPlans, 
      [CanBeNull] BuildPlansCollection runtimeBuildPlans)
    {
      if (buildStages == null) throw new ArgumentNullException("buildStages");
      if (buildPlans == null) throw new ArgumentNullException("buildPlans");
      _buildStages = buildStages;
      _buildPlans = buildPlans;
      _runtimeBuildPlans = runtimeBuildPlans;
      _buildSequence = new List<UnitInfo>(4);
    }

    [CanBeNull, Pure]
    public BuildResult BuildUnit([NotNull] UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      
      using (LogBuildSessionState(unitInfo))
      {
        _buildSequence.Add(unitInfo);
        try
        {
          var actions = _buildPlans.GetBuildActions(_buildSequence);
          var runtimeActions = _runtimeBuildPlans == null ? null : _runtimeBuildPlans.GetBuildActions(_buildSequence);
          return BuildUnit(actions.Merge(runtimeActions));
        }
        finally
        {
          _buildSequence.RemoveAt(_buildSequence.Count - 1);
        }
      }
    }

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

        Log.Info("");
        Log.Info("Execute: [{0}], [{1}]", buildAction, stage);
        
        performedActions.Push(buildAction);
        buildAction.Process(unitBuilder);

        if (unitBuilder.BuildResult != null)
        {
          Log.Info("Unit is build: {0}", unitBuilder.BuildResult);
          break; // object is built, unwind called actions in reverse orders
        }
      }

      foreach (var buildAction in performedActions)
      {
        Log.Info("PostProcess: {0}", buildAction);
        buildAction.PostProcess(unitBuilder);
      }

      return unitBuilder.BuildResult;
    }

    private IDisposable LogBuildSessionState(UnitInfo unitInfo)
    {
      Log.Info("");
      Log.Info("BuildSession.Build UnitInfo={0}", unitInfo);
      var block = Log.AddIndent(true);
      {
        _buildSequence.LogBuildSequence();
        Log.Info("");
        return block;
      }
    }
  }
}