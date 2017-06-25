using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Base class for a build step which triggers on the last unit in the build sequence only.
  /// Returns no build action if does not match building unit
  /// </summary>
  public abstract class LeafBuildStep : BuildStepBase
  {
    private readonly int _weight;

    protected LeafBuildStep(int weight)
    {
      _weight = weight;
    }

    public override MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
    {
      if (buildSequence.Length != 1) return null;

      var buildAction = GetBuildAction(buildSequence.GetLastItem());

      return buildAction == null
        ? null
        : new MatchedBuildActions{{buildAction.BuildStage, new List<Weighted<IBuildAction>>{buildAction.BuildAction.WithWeight(inputWeight + _weight)}}};
    }

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