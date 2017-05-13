using Armature.Common;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyUnitBuildStep : BuildStep
  {
    public override MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
    {
      return GetChildrenActions(inputWeight + PassingBuildSequenceWeight.AnyUnit, buildSequence.GetLastItemAsTail())
        .Merge(GetOwnActions(inputWeight + PassingBuildSequenceWeight.AnyUnit));
    }

    public override IBuildStep GetChildBuldStep(ArrayTail<IBuildStep> buildStepsSequence)
    {
      return null;
    }

    public override bool Equals(IBuildStep other)
    {
      return other is AnyUnitBuildStep;
    }
  }
}