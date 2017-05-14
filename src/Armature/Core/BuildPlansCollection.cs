using System;
using System.Collections.Generic;
using Armature.Common;

namespace Armature.Core
{
  public class BuildPlansCollection
  {
    /// <summary>
    /// Reuse implementation of <see cref="BuildStepBase"/> to implement <see cref="BuildPlansCollection"/> public interface
    /// </summary>
    private readonly RootBuildStep _rootBuildStep = new RootBuildStep();

    public MatchedBuildActions GetBuildActions(IList<UnitInfo> buildSequence)
    {
      return _rootBuildStep.GetBuildActions(0, ArrayTail.Of(buildSequence, 0));
    }

    public void AddBuildStep(IBuildStep buildStep)
    {
      _rootBuildStep.AddBuildStep(buildStep);
    }

    public IEnumerable<IBuildStep> Children
    {
      get { return _rootBuildStep.Children; }
    }

    private class RootBuildStep : BuildStepBase
    {
      public override MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
      {
        return GetChildrenActions(inputWeight, buildSequence);
      }

      public override bool Equals(IBuildStep other)
      {
        throw new NotSupportedException();
      }
    }
  }
}