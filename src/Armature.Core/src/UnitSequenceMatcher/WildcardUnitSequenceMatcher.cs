using System;
using System.Diagnostics;
using Armature.Core.Common;


namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Moves along the building units sequence from left to right skipping units until it encounters a matching unit. Behaves like string search with wildcard.
  /// </summary>
  public class WildcardUnitSequenceMatcher : UnitSequenceMatcherWithChildren, IEquatable<WildcardUnitSequenceMatcher>
  {
    private readonly IUnitIdMatcher _matcher;

    public WildcardUnitSequenceMatcher(IUnitIdMatcher matcher) : this(matcher, UnitSequenceMatchingWeight.WildcardMatchingUnit) { }

    public WildcardUnitSequenceMatcher(IUnitIdMatcher matcher, int weight) : base(weight)
      => _matcher = matcher ?? throw new ArgumentNullException(nameof(matcher));

    /// <summary>
    ///   Moves along the unit building sequence from left to right skipping units until it encounters a matching unit.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override MatchedBuildActions? GetBuildActions(ArrayTail<UnitId> buildingUnitsSequence, int inputWeight)
    {
      var realWeight = inputWeight;

      for(var i = 0; i < buildingUnitsSequence.Length; i++)
      {
        var unitInfo = buildingUnitsSequence[i];

        if(_matcher.Matches(unitInfo))
          return GetActions(buildingUnitsSequence.GetTail(i), realWeight);

        // increase weight on each "skipping" step, it will lead that "deeper" context has more weight then more common
        // it is needed when some Unit is registered several times, Unit under construction should receive that one which is "closer" to it
        realWeight++;
      }

      return null;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>.{2}", GetType().Name, Weight, _matcher);

    #region Equality

    public bool Equals(WildcardUnitSequenceMatcher? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Equals(_matcher, other._matcher) && Weight == other.Weight;
    }

    public override bool Equals(IUnitSequenceMatcher other) => Equals(other as WildcardUnitSequenceMatcher);

    public override bool Equals(object obj) => Equals(obj as WildcardUnitSequenceMatcher);

    public override int GetHashCode()
    {
      unchecked
      {
        return (_matcher.GetHashCode() * 397) ^ Weight;
      }
    }

    #endregion
  }
}
