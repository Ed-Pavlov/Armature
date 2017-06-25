using System;
using System.Collections.Generic;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents whole build session of one Unit, all dependency of the built unit are built in context of one build session
  /// </summary>
  public class BuildSession
  {
    private readonly IEnumerable<object> _buildStages;
    private readonly BuildPlansCollection _buildPlans;
    private readonly List<UnitInfo> _buildSequence = new List<UnitInfo>(4);
    
    [CanBeNull] 
    private readonly BuildPlansCollection _sessionBuildPlans;

    /// <summary>
    /// Represents a full build session, building an Unit and all its dependencies
    /// </summary>
    /// <param name="buildStages">The conveyer of build stages. <see cref="Builder"/> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="sessionBuildPlans">Sessional build plans used to build a unit. Sessional plans overrides common <paramref name="buildPlans"/>
    /// for this build session</param>
    public BuildSession(
      [NotNull] IEnumerable<object> buildStages, 
      [NotNull] BuildPlansCollection buildPlans, 
      [CanBeNull] BuildPlansCollection sessionBuildPlans)
    {
      if (buildStages == null) throw new ArgumentNullException("buildStages");
      if (buildPlans == null) throw new ArgumentNullException("buildPlans");
      _buildStages = buildStages;
      _buildPlans = buildPlans;
      _sessionBuildPlans = sessionBuildPlans;
    }

    /// <summary>
    /// Builds a Unit represented by <paramref name="unitInfo"/>
    /// </summary>
    /// <param name="unitInfo">"Id" of the unit to build. <see cref="IBuildStep"/> for details</param>
    [CanBeNull]
    public BuildResult BuildUnit([NotNull] UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      
      BuildResult result = null;

      using (LogBuildSessionState(unitInfo))
      {
        _buildSequence.Add(unitInfo);
        try
        {
          if (_sessionBuildPlans != null)
            result = BuildUnit(unitInfo, _sessionBuildPlans.GetBuildActions(_buildSequence));

          if(result == null)
            result = BuildUnit(unitInfo, _buildPlans.GetBuildActions(_buildSequence));
        }
        finally
        {
          _buildSequence.RemoveAt(_buildSequence.Count - 1);
        }
      }

      return result;
    }

    private BuildResult BuildUnit(UnitInfo unitInfo, MatchedBuildActions matchedBuildActions)
    {
      if (matchedBuildActions == null) return null;

      var unitBuilder = new UnitBuilder(unitInfo, _buildSequence, this);
      var performedActions = new Stack<IBuildAction>();
      foreach (var stage in _buildStages)
      {
        var buildAction = matchedBuildActions.GetTopmostAction(stage);
        if (buildAction == null)
          continue;

        Log.Info("[{0}] {1}", stage, buildAction);
        
        performedActions.Push(buildAction);
        buildAction.Execute(unitBuilder);

        if (unitBuilder.BuildResult != null)
          break; // object is built, unwind called actions in reverse orders
      }

      foreach (var action in performedActions)
        action.PostProcess(unitBuilder);

      return unitBuilder.BuildResult;
    }

    private IDisposable LogBuildSessionState(UnitInfo unitInfo)
    {
      Log.Info("");
      Log.Info("BuildSession.Build");
      var block = Log.AddIndent(true);
      {
        Log.Info("UnitInfo={0}", unitInfo);
        _buildSequence.LogBuildSequence();
        return block;
      }
    }
  }
}