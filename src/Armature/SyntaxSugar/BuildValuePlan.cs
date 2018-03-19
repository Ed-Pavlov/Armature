using System;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.UnitSequenceMatcher;
using Armature.Properties;

namespace Armature
{
  public abstract class BuildValuePlan : IBuildPlan
  {
    private readonly IBuildAction _getValueAction;
    private readonly IUnitMatcher _unitMatcher;
    private readonly int _weight;
    
    protected BuildValuePlan([NotNull] IUnitMatcher unitMatcher, [NotNull] IBuildAction getValueAction, int weight)
    {
      if (unitMatcher is null) throw new ArgumentNullException(nameof(unitMatcher));
      if (getValueAction is null) throw new ArgumentNullException(nameof(getValueAction));

      _unitMatcher = unitMatcher;
      _weight = weight;
      _getValueAction = getValueAction;
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