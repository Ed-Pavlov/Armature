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
    private Dictionary<object, List<IBuildAction>>? _buildActions;
    private Dictionary<object, List<IBuildAction>>  LazyBuildAction => _buildActions ??= new Dictionary<object, List<IBuildAction>>();

    protected PatternTreeNode(int weight) => Weight = weight;

    public abstract ICollection<IPatternTreeNode> Children { get; }
    public abstract BuildActionBag?               GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight);

    /// <inheritdoc />
    [DebuggerStepThrough]
    public virtual IPatternTreeNode UseBuildAction(IBuildAction buildAction, object buildStage)
    {
      LazyBuildAction
       .GetOrCreateValue(buildStage, () => new List<IBuildAction>())
       .Add(buildAction);

      return this;
    }

    protected int Weight { [DebuggerStepThrough] get; }

    [DebuggerStepThrough]
    protected BuildActionBag? GetOwnBuildActions(int matchingWeight)
    {
      if(_buildActions is null) return null;

      var result = new BuildActionBag();

      foreach(var pair in _buildActions)
        result.Add(pair.Key, pair.Value.Select(_ => _.WithWeight(matchingWeight)).ToList());

      return result;
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}<{1:n0}>", GetType().GetShortName(), Weight);

    public abstract bool Equals(IPatternTreeNode other);

    public void PrintToLog()
    {
      ICollection<IPatternTreeNode>? children = null;

      try
      {
        children = Children;
      }
      catch
      { // do nothing
      }

      if(children is not null)
        foreach(var child in children)
          using(Log.Block(LogLevel.Info, child.ToString()))
          {
            child.PrintToLog();
          }

      if(_buildActions is not null)
        using(Log.Block(LogLevel.Info, "Build actions"))
        {
          foreach(var pair in _buildActions)
            using(Log.Block(LogLevel.Info, "Stage: {0}", pair.Key))
            {
              foreach(var buildAction in pair.Value)
                if(buildAction is ILogable printable)
                  printable.PrintToLog();
                else
                  Log.Info(buildAction.ToString());
            }
        }
    }

    #region Syntax sugar

    public void             Add(IPatternTreeNode patternTreeNode) => Children.Add(patternTreeNode);
    IEnumerator IEnumerable.GetEnumerator()                       => throw new NotSupportedException();

    #endregion
  }
}
