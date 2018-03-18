using System;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.Properties;
using JetBrains.Annotations;

namespace Armature
{
  public interface IPropertyValueBuildPlan : IBuildPlan
  {
  }

  public class PropertyValueBuildPlan : BuildValuePlan, IPropertyValueBuildPlan
  {
    private readonly IBuildAction _getPropertyAction;

    public PropertyValueBuildPlan([NotNull] IUnitMatcher propertyMatcher, [NotNull] IBuildAction getPropertyAction, [NotNull] IBuildAction getValueAction, int weight)
      : base(propertyMatcher, getValueAction, weight) => 
      _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    protected override void Register(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance, 0))
        .AddBuildAction(BuildStage.Create, _getPropertyAction);
  }
}