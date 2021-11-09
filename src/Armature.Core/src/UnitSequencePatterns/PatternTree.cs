using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

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
///       new IfFirstUnitMatches(new ConstructorPattern(), 0)
///         .AddBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
///       new IfFirstUnitMatches(new MethodArgumentPattern(), ParameterMatchingWeight.Lowest)
///         .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
///     }
///   };
/// </remarks>
public class PatternTree : IPatternTreeNode, IEnumerable, ILogPrintable
{
  private readonly Root _root = new();

  public ICollection<IPatternTreeNode> Children => _root.Children;

  public WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight = 0)
    => _root.GatherBuildActions(unitSequence, 0);

  public void PrintToLog(LogLevel logLevel = LogLevel.None) => _root.PrintToLog(logLevel);

  public BuildActionBag BuildActions                   => throw new NotSupportedException();
  public bool           Equals(IPatternTreeNode other) => throw new NotSupportedException();

  #region Syntax sugar

  public void             Add(IPatternTreeNode patternTreeNode) => Children.Add(patternTreeNode);
  IEnumerator IEnumerable.GetEnumerator()                       => throw new NotSupportedException();

  #endregion

  /// <summary>
  ///   Reuse implementation of <see cref="PatternTreeNodeWithChildrenBase" /> to implement <see cref="PatternTree" /> public interface
  /// </summary>
  private class Root : PatternTreeNodeBase
  {
    [DebuggerStepThrough]
    public Root() : base(0) { }

    [DebuggerStepThrough]
    public override WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight)
    {
      if(RawChildren is null) return null;

      WeightedBuildActionBag? result = null;
      foreach(var child in RawChildren)
        result = result.Merge(child.GatherBuildActions(unitSequence, inputWeight));
      return result;
    }

    [DebuggerStepThrough]
    public override bool Equals(IPatternTreeNode? other) => throw new NotSupportedException();
  }
}