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
    private readonly IUnitPattern _unitPattern;

    [DebuggerStepThrough]
    public IfLastUnitMatches(IUnitPattern unitPattern, int weight = QueryWeight.Any) : base(weight)
      => _unitPattern = unitPattern ?? throw new ArgumentNullException(nameof(unitPattern));

    public override ICollection<IPatternTreeNode> Children => throw new NotSupportedException("This pattern can't contain children");

    public override BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      if(unitSequence.Length > 1) return null;

      if(!_unitPattern.Matches(unitSequence.Last()))
      {
        Log.WriteLine(LogLevel.Trace, () => string.Format("{0}{{not matched}}", this));
        return null;
      }

      using(Log.Block(LogLevel.Verbose, ToString)) // pass method group, do not call ToString
      {
        var buildActions = GetOwnBuildActions(Weight + inputWeight);
        buildActions.ToLog();

        return buildActions;
      }
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>.{2}", GetType().GetShortName(), Weight, _unitPattern);

    #region Equality

    private bool Equals(IfLastUnitMatches? other)
    {
      if(ReferenceEquals(null, other)) return false;
      if(ReferenceEquals(this, other)) return true;

      return Weight == other.Weight && _unitPattern.Equals(other._unitPattern);
    }

    public override bool Equals(IPatternTreeNode obj) => Equals(obj as IfLastUnitMatches);

    public override bool Equals(object obj) => Equals(obj as IfLastUnitMatches);

    public override int GetHashCode()
    {
      unchecked
      {
        return (Weight * 397) ^ _unitPattern.GetHashCode();
      }
    }

    #endregion
  }
}
