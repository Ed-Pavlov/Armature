using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class implementing the logic of adding build actions
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
  public abstract class PatternTreeNode : IPatternTreeNode, IEnumerable
  {
    private BuildActionBag? _buildActions;
    private BuildActionBag  LazyBuildAction => _buildActions ??= new BuildActionBag();

    protected PatternTreeNode(int weight) => Weight = weight;

    public virtual  BuildActionBag                BuildActions => LazyBuildAction;
    public abstract ICollection<IPatternTreeNode> Children     { get; }
    public abstract WeightedBuildActionBag?       GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight);

    protected int Weight { [DebuggerStepThrough] get; }

    protected WeightedBuildActionBag? GetOwnBuildActions(int matchingWeight)
    {
      if(_buildActions is null) return null;

      var result = new WeightedBuildActionBag();

      foreach(var pair in _buildActions)
        result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

      return result;
    }

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().GetShortName()}{{ {Weight:n0} }}";

    public virtual bool Equals(IPatternTreeNode? other)
      => other is PatternTreeNode otherNode && Weight == otherNode.Weight && GetType() == otherNode.GetType();

    public override bool Equals(object? obj) => Equals(obj as IPatternTreeNode);

    public override int GetHashCode() => Weight.GetHashCode();

    public void PrintToLog()
    {
      ICollection<IPatternTreeNode>? children = null;

      try
      {
        children = Children;
      }
      catch
      { // access to Children could throw an exception, do nothing
      }

      if(children is not null)
        foreach(var child in children)
          using(Log.Block(LogLevel.Info, child.ToString))
          {
            child.PrintToLog();
          }

      if(_buildActions is not null)
        using(Log.Block(LogLevel.Info, "Build actions"))
        {
          foreach(var pair in _buildActions)
            using(Log.Block(LogLevel.Info, $"Stage: {pair.Key}"))
            {
              foreach(var buildAction in pair.Value)
                if(buildAction is ILogable printable)
                  printable.PrintToLog();
                else
                  Log.WriteLine(LogLevel.Info, buildAction.ToString);
            }
        }
    }

    #region Syntax sugar

    public void             Add(IPatternTreeNode patternTreeNode) => Children.Add(patternTreeNode);
    IEnumerator IEnumerable.GetEnumerator()                       => throw new NotSupportedException();

    #endregion
  }
}
