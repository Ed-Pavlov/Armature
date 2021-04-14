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
    protected BuildValuePlan(IUnitIdMatcher unitMatcher, IBuildAction getValueAction, int weight)
      : base(unitMatcher, getValueAction, weight) { }

    void IBuildPlan.Apply(IScannerTree scannerTree)
    {
      scannerTree
       .AddItem(new IfLastUnitIs(UnitMatcher, Weight), false)
       .AddBuildAction(BuildStage.Create, BuildAction);

      Apply(scannerTree);
    }

    /// <summary>
    ///   Can be overriden to add extra logic in addition to implemented in <see cref="IBuildPlan.Apply" />
    /// </summary>
    protected virtual void Apply(IScannerTree scannerTree) { }
  }
}
