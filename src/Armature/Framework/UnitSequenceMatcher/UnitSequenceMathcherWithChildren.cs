using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature.Common;
using Armature.Core;
using Armature.Logging;
using Armature.Properties;

namespace Armature.Framework.UnitSequenceMatcher
{
  /// <summary>
  ///   Base class exposing the collection of children matchers, gathering and merging build actions from children with its own.
  /// </summary>
  public abstract class UnitSequenceMathcherWithChildren : UnitSequenceMatcher
  {
    private HashSet<IUnitSequenceMatcher> _children;

    protected UnitSequenceMathcherWithChildren(int weight) : base(weight)
    {
    }

    private HashSet<IUnitSequenceMatcher> LazyChildren{[DebuggerStepThrough] get => _children ?? (_children = new HashSet<IUnitSequenceMatcher>()); }
    
    [NotNull]
    public override ICollection<IUnitSequenceMatcher> Children { [DebuggerStepThrough] get { return LazyChildren; } }

    /// <summary>
    ///   Gets and merges matched actions from all children matchers
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which used by children build steps to calculate a final weight of matching</param>
    /// <param name="unitBuildingSequence">The sequence of unit infos to match with build steps and find suitable one</param>
    [DebuggerStepThrough]
    [CanBeNull]
    protected MatchedBuildActions GetChildrenActions(int inputMatchingWeight, ArrayTail<UnitInfo> unitBuildingSequence) =>
      _children?.Aggregate(
        (MatchedBuildActions)null,
        (current, child) => current.Merge(child.GetBuildActions(unitBuildingSequence, inputMatchingWeight)));

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    protected MatchedBuildActions GetActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight)
    {
      MatchedBuildActions matchedBuildActions;
      if (buildingUnitsSequence.Length > 1)
      {
        Log.Verbose(this.ToString());
        using (Log.AddIndent())
          matchedBuildActions = GetChildrenActions(inputWeight + Weight, buildingUnitsSequence.GetTail(1)); // pass the rest of the sequence to children and return their actions
      }
      else
      {
        matchedBuildActions = GetOwnActions(inputWeight);
        if (matchedBuildActions == null)
          Log.Trace("{0}{{not matched}}", this);
        else
          using (Log.Block(LogLevel.Verbose, this.ToString()))
            matchedBuildActions.ToLog();
      }

      return matchedBuildActions;
    }
  }
}