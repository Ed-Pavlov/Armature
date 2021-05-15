using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  public class SkipWhileUnit : PatternTreeNodeWithChildren
  {
    private readonly IUnitPattern _pattern;

    public SkipWhileUnit(IUnitPattern unitPattern, int weight = 0) : base(weight)
      => _pattern = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));


    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      var i = 0;

      for(; i < unitSequence.Length - 1; i++)
        if(!_pattern.Matches(unitSequence[i]))
          break;

      return GetChildrenActions(unitSequence.GetTail(i), inputWeight);
    }
    
    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().GetShortName()}( {_pattern.ToLogString()} ){{ Weight={Weight:n0} }}";
    
    #region Equality

    public bool Equals(SkipWhileUnit? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Equals(_pattern, other._pattern) && Weight == other.Weight;
    }

    public override bool Equals(IPatternTreeNode? other) => Equals(other as SkipWhileUnit);

    public override bool Equals(object? obj) => Equals(obj as SkipWhileUnit);

    public override int GetHashCode()
    {
      unchecked
      {
        return (_pattern.GetHashCode() * 397) ^ Weight;
      }
    }

    #endregion
  }
}
