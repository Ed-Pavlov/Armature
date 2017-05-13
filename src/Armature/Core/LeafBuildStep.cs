using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Base class for a build step which triggers on the last unit in the build sequence only.
  /// Returns no build action if does not match building unitl
  /// Can't be a parent for any other build steps.
  /// Always returns false for <see cref="Equals"/> and null for <see cref="GetChildBuldStep"/> because these
  /// methods are used to make a chain for build steps.
  /// </summary>
  public abstract class LeafBuildStep : IBuildStep
  {
    private readonly int _weight;

    protected LeafBuildStep(int weight)
    {
      _weight = weight;
    }

    public MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
    {
      if (buildSequence.Length != 1) return null;

      var buildStep = GetBuildAction(buildSequence.GetLastItem());

      return buildStep == null
        ? null
        : new MatchedBuildActions{{buildStep.BuildStage, new List<Weighted<IBuildAction>>{buildStep.BuildAction.WithWeight(inputWeight + _weight)}}};
    }

    protected abstract StagedBuildAction GetBuildAction(UnitInfo unitInfo);

    public IBuildStep GetChildBuldStep(ArrayTail<IBuildStep> buildStepsSequence)
    {
      return null;
    }

    public bool Equals(IBuildStep other)
    {
      return false;
    }

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