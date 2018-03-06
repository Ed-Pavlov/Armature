using Armature.Common;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyUnitSequenceMatcher : UnitSequenceMatcherBase
  {
    /// <summary>
    /// Matches any <see cref="UnitInfo"/>, so it pass the building unit info into its children and returns merged result
    /// </summary>
    public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputMatchingWeight)
    {
      var lastItemAsTail = buildingUnitsSequence.GetTail(buildingUnitsSequence.Length - 1);

      return GetChildrenActions(inputMatchingWeight + UnitSequenceMatchingWeight.AnyUnit, lastItemAsTail)
        .Merge(GetOwnActions(lastItemAsTail[0], inputMatchingWeight + UnitSequenceMatchingWeight.AnyUnit));
    }

    #region Equality

    public override bool Equals(IUnitSequenceMatcher other)
    {
      return Equals((object)other);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj is AnyUnitSequenceMatcher;
    }

    public override int GetHashCode()
    {
      return 0;
    }

    public static bool operator ==(AnyUnitSequenceMatcher left, AnyUnitSequenceMatcher right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(AnyUnitSequenceMatcher left, AnyUnitSequenceMatcher right)
    {
      return !Equals(left, right);
    }

    #endregion
  }
}