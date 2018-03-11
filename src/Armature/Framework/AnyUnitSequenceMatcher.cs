using System.Diagnostics;
using Armature.Common;
using Armature.Core;

namespace Armature.Framework
{
  public class AnyUnitSequenceMatcher : UnitSequenceMatcherBase
  {
    /// <summary>
    ///   Matches any <see cref="UnitInfo" />, so it pass the building unit info into its children and returns merged result
    /// </summary>
    public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputMatchingWeight)
    {
      var lastItemAsTail = buildingUnitsSequence.GetTail(buildingUnitsSequence.Length - 1);

      return GetChildrenActions(inputMatchingWeight + UnitSequenceMatchingWeight.AnyUnit, lastItemAsTail)
        .Merge(GetOwnActions(lastItemAsTail[0], inputMatchingWeight + UnitSequenceMatchingWeight.AnyUnit));
    }

    #region Equality
    [DebuggerStepThrough]
    public override bool Equals(IUnitSequenceMatcher other) => Equals((object)other);

    //TODO: is it right equality logic for it? how about different children?
    [DebuggerStepThrough]
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is AnyUnitSequenceMatcher;
    }

    public override int GetHashCode() => 0;

    [DebuggerStepThrough]
    public static bool operator ==(AnyUnitSequenceMatcher left, AnyUnitSequenceMatcher right) => Equals(left, right);

    [DebuggerStepThrough]
    public static bool operator !=(AnyUnitSequenceMatcher left, AnyUnitSequenceMatcher right) => !Equals(left, right);
    #endregion
  }
}