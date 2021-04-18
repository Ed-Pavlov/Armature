using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  /// <summary>
  ///   Stores and applies a plan how to build value to inject.
  /// </summary>
  public abstract class BuildValuePlan : BuildActionExtensibility, IBuildPlan
  {
    protected BuildValuePlan(IUnitIdPattern unitPattern, IBuildAction getValueAction, int weight)
      : base(unitPattern, getValueAction, weight) { }

    void IBuildPlan.Apply(IPatternTreeNode patternTreeNode)
    {
      patternTreeNode
       .AddNode(new IfLastUnitMatches(UnitPattern, Weight))
       .UseBuildAction(BuildStage.Create, BuildAction);

      Apply(patternTreeNode);
    }

    /// <summary>
    ///   Can be overriden to add extra logic in addition to implemented in <see cref="IBuildPlan.Apply" />
    /// </summary>
    protected virtual void Apply(IPatternTreeNode patternTreeNode) { }
  }
}
