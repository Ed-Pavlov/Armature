using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class exposing the collection of children nodes, gathering, and merging build actions from children.
  /// </summary>
  /// <remarks>
  /// This class implements <see cref="IEnumerable" /> and has <see cref="Add" /> method in order to make possible compact and readable initialization like
  /// new FooNode
  /// {
  ///   new SubNode(Pattern1.Instance, 0)
  ///     .UseBuildAction(BuildStage.Create, new GetLongestConstructorBuildAction()),
  ///   new SubNode(Pattern2.Instance, ParameterMatchingWeight.Lowest)
  ///     .AddBuildAction(BuildStage.Create, new RedirectParameterInfoBuildAction())
  /// };
  /// </remarks>
  public abstract class PatternTreeNodeWithChildrenBase : IPatternTreeNode, IEnumerable
  {
    private HashSet<IPatternTreeNode>? _children;
    private HashSet<IPatternTreeNode>  LazyChildren => _children ??= new HashSet<IPatternTreeNode>();

    protected PatternTreeNodeWithChildrenBase(int weight) => Weight = weight;

    public abstract WeightedBuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight);
    public abstract BuildActionBag          BuildActions { get; }

    public ICollection<IPatternTreeNode> Children => LazyChildren;

    protected int Weight { [DebuggerStepThrough] get; }

    /// <summary>
    ///   Gathers and merges build actions from all children nodes.
    /// </summary>
    /// <param name="unitSequence">The sequence of units building in this build session.</param>
    /// <param name="inputMatchingWeight">The weight of matching which passed to children to calculate a final weight of matching.</param>
    protected WeightedBuildActionBag? GetChildrenActions(ArrayTail<UnitId> unitSequence, int inputMatchingWeight)
      => _children?.Aggregate(
        (WeightedBuildActionBag?)null,
        (current, child) => current.Merge(child.GatherBuildActions(unitSequence, inputMatchingWeight)));

    public virtual void PrintToLog()
    {
      if(_children is not null)
        foreach(var child in _children)
          using(Log.Block(LogLevel.Info, child.ToString))
            child.PrintToLog();
    }

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().GetShortName()}{{ {Weight:n0} }}";

    public virtual bool Equals(IPatternTreeNode? other)
      => other is PatternTreeNodeBase otherNode && Weight == otherNode.Weight && GetType() == otherNode.GetType();

    public override bool Equals(object? obj) => Equals(obj as IPatternTreeNode);

    public override int GetHashCode() => Weight.GetHashCode();

    #region Syntax sugar

    public void             Add(IPatternTreeNode patternTreeNode) => Children.Add(patternTreeNode);
    IEnumerator IEnumerable.GetEnumerator()                       => throw new NotSupportedException();

    #endregion
  }
}
