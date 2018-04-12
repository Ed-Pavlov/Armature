using System;
using Armature.Core;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public class PropertyValueBuildPlan : BuildValuePlan, IPropertyValueBuildPlan
  {
    private readonly IBuildAction _getPropertyAction;

    public PropertyValueBuildPlan(
      [NotNull] IUnitMatcher propertyMatcher,
      [NotNull] IBuildAction getPropertyAction,
      [NotNull] IBuildAction getValueAction,
      int weight)
      : base(propertyMatcher, getValueAction, weight) =>
      _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    protected override void Register(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, _getPropertyAction);
  }
}