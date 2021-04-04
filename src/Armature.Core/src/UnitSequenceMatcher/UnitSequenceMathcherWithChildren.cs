using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature.Core.Common;
using Armature.Core.Logging;


namespace Armature.Core.UnitSequenceMatcher
{
  /// <summary>
  ///   Base class exposing the collection of children matchers, gathering and merging build actions from children with its own.
  /// </summary>
  public abstract class UnitSequenceMatcherWithChildren : UnitSequenceMatcher
  {
    private HashSet<IUnitSequenceMatcher>? _children;

    protected UnitSequenceMatcherWithChildren(int weight) : base(weight) { }

    private HashSet<IUnitSequenceMatcher> LazyChildren { [DebuggerStepThrough] get => _children ??= new HashSet<IUnitSequenceMatcher>(); }

    public override ICollection<IUnitSequenceMatcher> Children { [DebuggerStepThrough] get => LazyChildren; }

    /// <summary>
    ///   Gets and merges matched actions from all children matchers
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
    /// <param name="unitBuildingSequence">The sequence of units building in this build session.</param>
    [DebuggerStepThrough]
    protected MatchedBuildActions? GetChildrenActions(int inputMatchingWeight, ArrayTail<UnitInfo> unitBuildingSequence) =>
      _children?.Aggregate(
        (MatchedBuildActions?)null,
        (current, child) => current.Merge(child.GetBuildActions(unitBuildingSequence, inputMatchingWeight)));

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    protected MatchedBuildActions? GetActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      MatchedBuildActions? matchedBuildActions;
      if (buildingUnitsSequence.Length > 1)
      {
        Log.WriteLine(LogLevel.Verbose, this.ToString);  // pass group method, do not call ToString
        using (Log.AddIndent())
        {
          matchedBuildActions = GetChildrenActions(
            inputWeight + Weight,
            buildingUnitsSequence.GetTail(1)); // pass the rest of the sequence to children and return their actions
        }
      }
      else
      {
        matchedBuildActions = GetOwnActions(Weight + inputWeight);
        if (matchedBuildActions == null)
          Log.WriteLine(LogLevel.Trace, () => string.Format("{0}{{not matched}}", this));
        else
          using (Log.Block(LogLevel.Verbose, this.ToString)) // pass group method, do not call ToString
          {
            // ReSharper disable once RedundantArgumentDefaultValue
            matchedBuildActions.ToLog(LogLevel.Verbose);
          }
      }

      return matchedBuildActions;
    }
  }
}