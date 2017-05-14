using System;
using System.Collections.Generic;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents whole build session of one Unit, all dependency of the built Unit are built in context of one build session
  /// </summary>
  public class Build
  {
    [NotNull] private readonly IEnumerable<object> _stages;
    [NotNull] private readonly BuildPlansCollection _buildPlans;
    [CanBeNull] private readonly BuildPlansCollection _sessionBuildPlans;

    private readonly List<UnitInfo> _buildSequence = new List<UnitInfo>(4);

    private Build([NotNull] IEnumerable<object> stages, [NotNull] BuildPlansCollection buildPlans, [CanBeNull] BuildPlansCollection sessionBuildPlans)
    {
      if (stages == null) throw new ArgumentNullException("stages");
      if (buildPlans == null) throw new ArgumentNullException("buildPlans");
      _stages = stages;
      _buildPlans = buildPlans;
      _sessionBuildPlans = sessionBuildPlans;
    }

    /// <summary>
    /// Builds a Unit represented by <paramref name="unitInfo"/>
    /// </summary>
    /// <param name="stages">The conveyer of build stages. <see cref="Builder"/> for details</param>
    /// <param name="unitInfo">"Id" of the unit to build. <see cref="IBuildStep"/> for details</param>
    /// <param name="buildPlans">Build plans used to build a unit</param>
    /// <param name="sessionBuildPlans">Sessional build plans used to build a unit. Sessional plans overrides common <paramref name="buildPlans"/></param>
    /// <returns>Returns <see cref="BuildUnit(Armature.Core.UnitInfo,MatchedBuildActions)"/> if unit was built or null otherwise. <see cref="BuildUnit(Armature.Core.UnitInfo,MatchedBuildActions)"/> for details.</returns>
    [CanBeNull]
    public static BuildResult BuildUnit(IEnumerable<object> stages, UnitInfo unitInfo, BuildPlansCollection buildPlans, BuildPlansCollection sessionBuildPlans)
    {
      return new Build(stages, buildPlans, sessionBuildPlans).BuildUnit(unitInfo);
    }

    [CanBeNull]
    private BuildResult BuildUnit([NotNull] UnitInfo unitInfo)
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

      var unitBuilder = new Session(unitInfo, _buildSequence, this);
      var performedActions = new Stack<IBuildAction>();
      foreach (var stage in _stages)
      {
        var action = matchedBuildActions.GetTopmostAction(stage);
        if (action == null)
          continue;

        performedActions.Push(action);
        action.Execute(unitBuilder);

        if (unitBuilder.BuildResult != null)
          break; // object is constructed, unwind called actions in reverse orders
      }

      foreach (var action in performedActions)
        action.PostProcess(unitBuilder);

      return unitBuilder.BuildResult;
    }

    public class Session
    {
      private readonly UnitInfo _unitInfo;
      private readonly Build _buildSession;
      private readonly IEnumerable<UnitInfo> _buildSequence;

      public Session(UnitInfo unitInfo, IEnumerable<UnitInfo> buildSequence, Build buildSession)
      {
        _unitInfo = unitInfo;
        _buildSession = buildSession;
        _buildSequence = buildSequence;
      }

      [CanBeNull]
      public BuildResult Build([NotNull] UnitInfo unitInfo)
      {
        return _buildSession.BuildUnit(unitInfo);
      }

      public UnitInfo UnitInfo { get { return _unitInfo; } }
      public BuildResult BuildResult { get; set; }
      public IEnumerable<UnitInfo> BuildSequence { get { return _buildSequence; } }
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