using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   The collection of build plans. Build plan of the unit is the tree of units sequence matchers containing build actions.
  ///   All build plans are contained as a forest of trees.
  ///   See <see cref="IPatternTreeNode" /> for details.
  /// </summary>
  /// <remarks>
  ///   This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  ///   new Builder(...)
  ///   {
  ///     new SkipToLastUnit
  ///     {
  ///       new IfLastUnitMatches(new ConstructorPattern(), 0)
  ///         .AddBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
  ///       new IfLastUnitMatches(new MethodArgumentPattern(), ParameterMatchingWeight.Lowest)
  ///         .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  ///     }
  ///   };
  /// </remarks>
  public class BuildPlanCollection : IPatternTreeNode, IEnumerable
  {
    private readonly TreeRoot _treeRoot = new();

    public ICollection<IPatternTreeNode> Children => _treeRoot.Children;
    
    public BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight = 0) => _treeRoot.GatherBuildActions(unitSequence, 0);

    public void PrintToLog()
    {
      using(Log.Enabled())
        _treeRoot.PrintToLog();
    }

    public bool                       Equals(IPatternTreeNode other)                                => throw new NotSupportedException();
    IPatternTreeNode IPatternTreeNode.UseBuildAction(object   buildStage, IBuildAction buildAction) => throw new NotSupportedException();

    #region Syntax sugar

    public void             Add(IPatternTreeNode patternTreeNode) => Children.Add(patternTreeNode);
    IEnumerator IEnumerable.GetEnumerator()                       => throw new NotSupportedException();

    #endregion

    /// <summary>
    ///   Reuse implementation of <see cref="PatternTreeNodeWithChildren" /> to implement <see cref="BuildPlanCollection" /> public interface
    /// </summary>
    private class TreeRoot : PatternTreeNodeWithChildren
    {
      [DebuggerStepThrough]
      public TreeRoot() : base(0) { }

      [DebuggerStepThrough]
      public override BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight) => GetChildrenActions(unitSequence, inputWeight);

      [DebuggerStepThrough]
      public override bool Equals(IPatternTreeNode other) => throw new NotSupportedException();
    }
  }
}
