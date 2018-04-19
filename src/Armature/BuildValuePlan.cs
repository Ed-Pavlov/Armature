using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;
using ArgumentNullException = System.ArgumentNullException;

namespace Armature
{
  /// <summary>
  /// Stores and applies a plan how to build value to inject.
  /// </summary>
  public abstract class BuildValuePlan : IBuildPlan
  {
    private readonly IBuildAction _getValueAction;
    private readonly IUnitMatcher _unitMatcher;
    private readonly int _weight;
    
    protected BuildValuePlan([NotNull] IUnitMatcher unitMatcher, [NotNull] IBuildAction getValueAction, int weight)
    {
      _unitMatcher = unitMatcher ?? throw new ArgumentNullException(nameof(unitMatcher));
      _getValueAction = getValueAction ?? throw new ArgumentNullException(nameof(getValueAction));
      _weight = weight;
    }
    
    void IBuildPlan.Apply(IUnitSequenceMatcher unitSequenceMatcher)
    {
      unitSequenceMatcher
        .AddUniqueUnitMatcher(new LastUnitSequenceMatcher(_unitMatcher, _weight))
        .AddBuildAction(BuildStage.Create, _getValueAction);
      Apply(unitSequenceMatcher);
    }

    /// <summary>
    ///  Can be overriden to add extra logic in addition to implemented in <see cref="IBuildPlan.Apply"/> 
    /// </summary>
    protected virtual void Apply(IUnitSequenceMatcher unitSequenceMatcher){}
  }
}