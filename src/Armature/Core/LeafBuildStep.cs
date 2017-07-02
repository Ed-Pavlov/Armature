using System;
using System.Collections.Generic;
using Armature.Common;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Base class for a build step which can match with the last unit in the build sequence only.
  /// Returns no build action if does not match building <see cref="UnitInfo"/>
  /// </summary>
  public abstract class LeafBuildStep : BuildStepBase
  {
    private readonly int _matchingWeight;

    protected LeafBuildStep(int matchingWeight)
    {
      _matchingWeight = matchingWeight;
    }

    public override MatchedBuildActions GetBuildActions(int inputMatchingWeight, ArrayTail<UnitInfo> matchingPattern)
    {
      if (matchingPattern.Length != 1) return null;

      using(Log.Block(GetType().Name, LogLevel.Trace))
      {
        var buildAction = GetBuildAction(matchingPattern.GetLastItem());
        if (buildAction == null)
          return null;

        var weightedBuildAction = buildAction.BuildAction.WithWeight(inputMatchingWeight + _matchingWeight);
        Log.Trace("build action {0}", weightedBuildAction);
        
        return new MatchedBuildActions
        {
          {
            buildAction.BuildStage,
            new List<WeightedBuildAction> {weightedBuildAction}
          }
        };
      }
    }

    //TODO: is it right, how to Remove such build step?
    public override bool Equals(IBuildStep obj)
    {
      return false;
    }

    protected abstract StagedBuildAction GetBuildAction(UnitInfo unitInfo);

    protected class StagedBuildAction
    {
      public readonly object BuildStage;
      public readonly IBuildAction BuildAction;

      public StagedBuildAction([NotNull] object buildStage, [NotNull] IBuildAction buildAction)
      {
        if (buildStage == null) throw new ArgumentNullException("buildStage");
        if (buildAction == null) throw new ArgumentNullException("buildAction");
        BuildStage = buildStage;
        BuildAction = buildAction;
      }
    }
  }
}