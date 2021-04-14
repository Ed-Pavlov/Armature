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
  public abstract class ScannerTreeWithChildren : ScannerTree
  {
    private HashSet<IScannerTree>? _children;

    protected ScannerTreeWithChildren(int weight) : base(weight) { }

    private HashSet<IScannerTree> LazyChildren
    {
      [DebuggerStepThrough] get => _children ??= new HashSet<IScannerTree>();
    }

    public override ICollection<IScannerTree> Children
    {
      [DebuggerStepThrough] get => LazyChildren;
    }

    /// <summary>
    ///   Gets and merges matched actions from all children matchers
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
    /// <param name="unitBuildingSequence">The sequence of units building in this build session.</param>
    [DebuggerStepThrough]
    protected BuildActionBag? GetChildrenActions(int inputMatchingWeight, ArrayTail<UnitId> unitBuildingSequence)
      => _children?.Aggregate(
        (BuildActionBag?) null,
        (current, child) => current.Merge(child.GetBuildActions(unitBuildingSequence, inputMatchingWeight)));

    [SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    protected BuildActionBag? GetActions(ArrayTail<UnitId> buildingUnitsSequence, int inputWeight)
    {
      BuildActionBag? BuildActionBag;

      if(buildingUnitsSequence.Length > 1)
      {
        Log.WriteLine(LogLevel.Verbose, this.ToString); // pass group method, do not call ToString

        using(Log.AddIndent())
        {
          BuildActionBag = GetChildrenActions(
            inputWeight + Weight,
            buildingUnitsSequence.GetTail(1)); // pass the rest of the sequence to children and return their actions
        }
      }
      else
      {
        BuildActionBag = GetOwnActions(Weight + inputWeight);

        if(BuildActionBag is null)
          Log.WriteLine(LogLevel.Trace, () => string.Format("{0}{{not matched}}", this));
        else
          using(Log.Block(LogLevel.Verbose, this.ToString)) // pass group method, do not call ToString
          {
            // ReSharper disable once RedundantArgumentDefaultValue
            BuildActionBag.ToLog(LogLevel.Verbose);
          }
      }

      return BuildActionBag;
    }
  }
}
