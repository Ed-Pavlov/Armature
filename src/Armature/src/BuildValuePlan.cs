using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;


namespace Armature
{
  /// <summary>
  ///   Stores and applies a plan how to build value to inject.
  /// </summary>
  public abstract class BuildValuePlan : BuildActionExtensibility, IBuildPlan
  {
    protected BuildValuePlan(IUnitMatcher unitMatcher, IBuildAction getValueAction, int weight)
      : base(unitMatcher, getValueAction, weight) { }

    void IBuildPlan.Apply(IUnitSequenceMatcher unitSequenceMatcher)
    {
      unitSequenceMatcher
       .AddUniqueUnitMatcher(new LastUnitSequenceMatcher(UnitMatcher, Weight))
       .AddBuildAction(BuildStage.Create, BuildAction);

      Apply(unitSequenceMatcher);
    }

    /// <summary>
    ///   Can be overriden to add extra logic in addition to implemented in <see cref="IBuildPlan.Apply" />
    /// </summary>
    protected virtual void Apply(IUnitSequenceMatcher unitSequenceMatcher) { }
  }
}
