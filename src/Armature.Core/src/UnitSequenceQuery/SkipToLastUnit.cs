using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Matches any sequence of building units, thus passing the unit under construction to <see cref="IPatternTreeNode.Children" /> and merge their
  ///   build actions with its own.
  /// </summary>
  public class SkipToLastUnit : PatternTreeNodeWithChildren
  {
    public SkipToLastUnit(int weight) : base(weight) { }
    public SkipToLastUnit() : this(QueryWeight.AnyUnit) { }

    public override IPatternTreeNode UseBuildAction(object buildStage, IBuildAction buildAction)
      => throw new NotSupportedException(
           "This query is used to skip unit sequence to the end and pass the unit under construction to sub queries."
         + "It can't contain build actions due to they are used to build the unit under construction only."
         );

    /// <summary>
    ///   Matches any <see cref="UnitId" />, so it pass the building unit info into its children and returns merged result
    /// </summary>
    public override BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      var unitsToSkipCount = unitSequence.Length;

      // decrease matching weight depending on how many unit in the sequence were skipped by this matcher
      var matchingWeight = inputWeight + Weight * unitsToSkipCount;

      using(Log.Block(LogLevel.Verbose, ToString, unitsToSkipCount)) // pass group method, do not call ToString method
      {
        var lastUnitAsTail = unitSequence.GetTail(unitSequence.Length - 1);
        return GetChildrenActions(matchingWeight, lastUnitAsTail);
      }
    }

    private string ToString(int unitsToSkip) => string.Format("{0}<x{1:n0}>", base.ToString(), unitsToSkip);

    #region Equality

    [DebuggerStepThrough]
    public override bool Equals(IPatternTreeNode other) => Equals((object) other);

    [DebuggerStepThrough]
    public override bool Equals(object? obj)
    {
      if(ReferenceEquals(null, obj)) return false;
      if(ReferenceEquals(this, obj)) return true;

      return obj is SkipToLastUnit other && Weight == other.Weight;
    }

    [DebuggerStepThrough]
    public override int GetHashCode() => Weight.GetHashCode();

    #endregion
  }
}
