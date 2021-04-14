using System;
using Armature.Core;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;


namespace Armature
{
  public class PropertyValueBuildPlan : BuildValuePlan, IPropertyValueBuildPlan
  {
    private readonly IBuildAction _getPropertyAction;

    public PropertyValueBuildPlan(
      IUnitIdMatcher propertyMatcher,
      IBuildAction getPropertyAction,
      IBuildAction getValueAction,
      int          weight)
      : base(propertyMatcher, getValueAction, weight)
      => _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    /// <summary>
    ///   In addition to the base logic adds a logic which provides a properties to inject into
    /// </summary>
    /// <param name="scannerTree"></param>
    protected override void Apply(IScannerTree scannerTree)
      => scannerTree
        .AddItem(
           new IfLastUnitIs(UnitIsPropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, _getPropertyAction);
  }
}
