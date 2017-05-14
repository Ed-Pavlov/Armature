using Armature.Common;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyUnitBuildStep : StaticBuildStep
  {
    public override MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
    {
      var lastItemAsTail = buildSequence.GetTail(buildSequence.Length - 1);

      return GetChildrenActions(inputWeight + PassingBuildSequenceWeight.AnyUnit, lastItemAsTail)
        .Merge(GetOwnActions(inputWeight + PassingBuildSequenceWeight.AnyUnit));
    }

    public override bool Equals(IBuildStep other)
    {
      return other is AnyUnitBuildStep;
    }
  }
}