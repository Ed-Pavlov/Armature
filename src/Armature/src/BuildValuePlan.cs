using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  /// <summary>
  ///   Stores and applies a plan how to build value to inject.
  /// </summary>
  public abstract class BuildValuePlan : BuildActionExtensibility, IBuildPlan
  {
    protected BuildValuePlan(IUnitIdMatcher unitMatcher, IBuildAction getValueAction, int weight)
      : base(unitMatcher, getValueAction, weight) { }

    void IBuildPlan.Apply(IQuery query)
    {
      query
       .AddSubQuery(new IfLastUnitIs(UnitMatcher, Weight), false)
       .UseBuildAction(BuildStage.Create, BuildAction);

      Apply(query);
    }

    /// <summary>
    ///   Can be overriden to add extra logic in addition to implemented in <see cref="IBuildPlan.Apply" />
    /// </summary>
    protected virtual void Apply(IQuery query) { }
  }
}
