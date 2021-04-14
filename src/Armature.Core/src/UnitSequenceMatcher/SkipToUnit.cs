using System;
using System.Diagnostics;
using Armature.Core.Common;


namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Moves along the building units sequence from left to right skipping units until it encounters a matching unit. Behaves like string search with wildcard.
  /// </summary>
  public class SkipToUnit : ScannerTreeWithChildren, IEquatable<SkipToUnit>
  {
    private readonly IUnitIdMatcher _matcher;

    public SkipToUnit(IUnitIdMatcher matcher) : this(matcher, UnitSequenceMatchingWeight.WildcardMatchingUnit) { }

    public SkipToUnit(IUnitIdMatcher matcher, int weight) : base(weight)
      => _matcher = matcher ?? throw new ArgumentNullException(nameof(matcher));

    /// <summary>
    ///   Moves along the unit building sequence from left to right skipping units until it encounters a matching unit.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override BuildActionBag? GetBuildActions(ArrayTail<UnitId> buildingUnitsSequence, int inputWeight)
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

    public bool Equals(SkipToUnit? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Equals(_matcher, other._matcher) && Weight == other.Weight;
    }

    public override bool Equals(IScannerTree other) => Equals(other as SkipToUnit);

    public override bool Equals(object obj) => Equals(obj as SkipToUnit);

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
