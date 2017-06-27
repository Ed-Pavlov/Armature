using Armature.Common;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyUnitBuildStep : StaticBuildStep
  {
    /// <summary>
    /// This steps matches any <see cref="UnitInfo"/>, so it pass the tail of <paramref name="matchingPattern"/> into its children and merges the result
    /// with its own build actions
    /// </summary>
    public override MatchedBuildActions GetBuildActions(int inputMatchingWeight, ArrayTail<UnitInfo> matchingPattern)
    {
      var lastItemAsTail = matchingPattern.GetTail(matchingPattern.Length - 1);

      return GetChildrenActions(inputMatchingWeight + UnitSequenceMatchingWeight.AnyUnit, lastItemAsTail)
        .Merge(GetOwnActions(inputMatchingWeight + UnitSequenceMatchingWeight.AnyUnit));
    }

    public override bool Equals(IBuildStep other)
    {
      return ReferenceEquals(this, other);
    }
  }
}