using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if the first unit in the building unit sequence matches the specified patter.
  /// </summary>
  public class IfFirstUnit : PatternTreeNodeWithChildren, IEquatable<IfFirstUnit>
  {
    private readonly IUnitPattern _pattern;

    public IfFirstUnit(IUnitPattern pattern, int weight = WeightOf.FirstUnit) : base(weight)
      => _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));

    /// <summary>
    ///   Checks if the first unit in the building unit sequence matches the specified patter.
    ///   If it is the unit under construction, returns build actions for it, if no, pass the rest of the sequence to each child and returns merged actions.
    /// </summary>
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
      => _pattern.Matches(unitSequence[0]) ? GetOwnOrChildrenBuildActions(unitSequence, inputWeight) : null;

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().GetShortName()}( {_pattern.ToLogString()} ){{ Weight={Weight:n0} }}";

    #region Equality

    public bool Equals(IfFirstUnit? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Equals(_pattern, other._pattern) && Weight == other.Weight;
    }

    public override bool Equals(IPatternTreeNode other) => Equals(other as IfFirstUnit);

    public override bool Equals(object obj) => Equals(obj as IfFirstUnit);

    public override int GetHashCode()
    {
      unchecked
      {
        return (_pattern.GetHashCode() * 397) ^ (int)Weight;
      }
    }

    #endregion
  }
}
