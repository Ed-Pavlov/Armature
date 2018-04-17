using System;
using System.Diagnostics;
using Resharper.Annotations;
using Armature.Core.Common;

namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Moves along the building units sequence from left to right skipping units until it encounters a matching unit. Behaves like string search with wildcard.
  ///  </summary>
  public class WildcardUnitSequenceMatcher : UnitSequenceMathcherWithChildren, IEquatable<WildcardUnitSequenceMatcher>
  {
    private readonly IUnitMatcher _matcher;

    public WildcardUnitSequenceMatcher([NotNull] IUnitMatcher matcher) : this(matcher, UnitSequenceMatchingWeight.WildcardMatchingUnit){}
    public WildcardUnitSequenceMatcher([NotNull] IUnitMatcher matcher, int weight) : base(weight) => 
      _matcher = matcher ?? throw new ArgumentNullException(nameof(matcher));

    /// <summary>
    ///   Moves along the unit building sequence from left to right skipping units until it encounters a matching unit.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      for (var i = 0; i < buildingUnitsSequence.Length; i++)
      {
        var unitInfo = buildingUnitsSequence[i];
        if (_matcher.Matches(unitInfo))
          return GetActions(buildingUnitsSequence.GetTail(i), inputWeight);
      }

      return null;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}.{1}", GetType().Name, _matcher);

    #region Equality
    public bool Equals(WildcardUnitSequenceMatcher other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;

      return Equals(_matcher, other._matcher) && Weight == other.Weight;
    }

    public override bool Equals(IUnitSequenceMatcher other) => Equals(other as WildcardUnitSequenceMatcher);

    public override bool Equals(object obj) => Equals(obj as WildcardUnitSequenceMatcher);

    public override int GetHashCode()
    {
      unchecked
      {
        return ((_matcher != null ? _matcher.GetHashCode() : 0) * 397) ^ Weight;
      }
    }
    #endregion
  }
}