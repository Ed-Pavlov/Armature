using System;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if a building unit sequence contains the only unit under construction and it matches the specified unit pattern.
  /// </summary>
  public class IfLastUnitMatches : PatternTreeNode
  {
    private readonly IUnitPattern _pattern;

    [DebuggerStepThrough]
    public IfLastUnitMatches(IUnitPattern unitPattern, int weight = 0) : base(weight)
      => _pattern = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));

    public override ICollection<IPatternTreeNode> Children => throw new NotSupportedException("This pattern can't contain children");

    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, long inputWeight)
    {
      if(unitSequence.Length > 1) return null;

      if(!_pattern.Matches(unitSequence.Last()))
      {
        Log.WriteLine(LogLevel.Trace, () => $"{this}{LogConst.NoMatch}");
        return null;
      }

      using(Log.Block(LogLevel.Trace, ToString)) // pass method group, do not call ToString
      {
        var buildActions = GetOwnBuildActions(Weight + inputWeight);
        buildActions.ToLog(LogLevel.Trace);

        return buildActions;
      }
    }

    [DebuggerStepThrough]
    public override string ToString() =>  $"{GetType().GetShortName()}( {_pattern.ToLogString()} ){{ Weight={Weight:n0} }}";

    #region Equality

    private bool Equals(IfLastUnitMatches? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Weight == other.Weight && _pattern.Equals(other._pattern);
    }

    public override bool Equals(IPatternTreeNode obj) => Equals(obj as IfLastUnitMatches);

    public override bool Equals(object obj) => Equals(obj as IfLastUnitMatches);

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
