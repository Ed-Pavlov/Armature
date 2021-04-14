using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.Logging;
using Armature.Core.UnitSequenceMatcher;


namespace Armature.Core
{
  /// <summary>
  ///   The collection of build plans. Build plan of the unit is the tree of units sequence matchers containing build actions.
  ///   All build plans are contained as a forest of trees.
  ///   See <see cref="IScannerTree" /> for details.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new Builder(...)
  ///   {
  ///     new AnyUnitSequenceMatcher
  ///     {
  ///       new LeafUnitSequenceMatcher(ConstructorMatcher.Instance, 0)
  ///         .AddBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
  ///       new LeafUnitSequenceMatcher(ParameterMatcher.Instance, ParameterMatchingWeight.Lowest)
  ///         .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  ///     }
  ///   };
  /// </remarks>
  public class BuildPlansCollection : IScannerTree, IEnumerable
  {
    private readonly Root _root = new();

    /// <summary>
    ///   Forest of <see cref="IScannerTree" /> trees
    /// </summary>
    public ICollection<IScannerTree> Children => _root.Children;

    public void Add(IScannerTree scannerTree) => Children.Add(scannerTree);

    public BuildActionBag? GetBuildActions(ArrayTail<UnitId> buildingUnitsSequence, int inputWeight = 0)
    {
      if(buildingUnitsSequence.Length == 0) throw new ArgumentException(nameof(buildingUnitsSequence));

      return _root.GetBuildActions(buildingUnitsSequence, 0);
    }

    public void PrintToLog()
    {
      using(Log.Enabled())
      {
        _root.PrintToLog();
      }
    }

    public bool Equals(IScannerTree other) => ReferenceEquals(this, other);

    /// <summary>
    ///   Reuse implementation of <see cref="ScannerTreeWithChildren" /> to implement <see cref="BuildPlansCollection" /> public interface
    /// </summary>
    private class Root : ScannerTreeWithChildren
    {
      [DebuggerStepThrough]
      public Root() : base(0) { }

      [DebuggerStepThrough]
      public override BuildActionBag? GetBuildActions(ArrayTail<UnitId> buildingUnitsSequence, int inputWeight)
        => GetChildrenActions(inputWeight, buildingUnitsSequence);

      [DebuggerStepThrough]
      public override bool Equals(IScannerTree other) => throw new NotSupportedException();
    }

    IEnumerator IEnumerable.  GetEnumerator()                                             => throw new NotSupportedException();
    IScannerTree IScannerTree.AddBuildAction(object buildStage, IBuildAction buildAction) => throw new NotSupportedException();
  }
}
