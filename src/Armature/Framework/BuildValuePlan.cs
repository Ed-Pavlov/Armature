using Armature.Core;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;
using ArgumentNullException = System.ArgumentNullException;

namespace Armature
{
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
    
    void IBuildPlan.Register(IUnitSequenceMatcher unitSequenceMatcher)
    {
      unitSequenceMatcher
        .AddUniqueUnitMatcher(new LastUnitSequenceMatcher(_unitMatcher, _weight))
        .AddBuildAction(BuildStage.Create, _getValueAction);
      Register(unitSequenceMatcher);
    }

    protected virtual void Register(IUnitSequenceMatcher unitSequenceMatcher){}
  }
}