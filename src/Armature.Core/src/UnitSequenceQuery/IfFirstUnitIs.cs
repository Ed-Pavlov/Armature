using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Matches the first unit in the sequence and only if it matches pass the tail of building
  ///   sequence to its <see cref="QueryWithChildren.Children" />
  /// </summary>
  public class IfFirstUnitIs : QueryWithChildren, IEquatable<IfFirstUnitIs>
  {
    private readonly IUnitIdMatcher _matcher;

    public IfFirstUnitIs(IUnitIdMatcher matcher) : this(matcher, QueryWeight.StrictMatchingUnit) { }

    public IfFirstUnitIs(IUnitIdMatcher matcher, int weight) : base(weight)
      => _matcher = matcher ?? throw new ArgumentNullException(nameof(matcher));

    /// <summary>
    ///   Moves along the unit building sequence from left to right skipping units until it encounters a matching unit.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      var unitInfo = unitSequence[0];

      return _matcher.Matches(unitInfo) ? GetActions(unitSequence, inputWeight) : null;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>.{2}", GetType().GetShortName(), Weight, _matcher);

    #region Equality

    public bool Equals(IfFirstUnitIs? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Equals(_matcher, other._matcher) && Weight == other.Weight;
    }

    public override bool Equals(IQuery other) => Equals(other as IfFirstUnitIs);

    public override bool Equals(object obj) => Equals(obj as IfFirstUnitIs);

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
