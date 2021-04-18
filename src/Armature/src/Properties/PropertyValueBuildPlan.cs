using System;
using Armature.Core;


namespace Armature
{
  public class PropertyValueBuildPlan : BuildValuePlan, IPropertyValueBuildPlan
  {
    private readonly IBuildAction _getPropertyAction;

    public PropertyValueBuildPlan(
      IUnitIdPattern propertyPattern,
      IBuildAction   getPropertyAction,
      IBuildAction   getValueAction,
      int            weight)
      : base(propertyPattern, getValueAction, weight)
      => _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    /// <summary>
    ///   In addition to the base logic adds a logic which provides a properties to inject into
    /// </summary>
    /// <param name="query"></param>
    protected override void Apply(IQuery query)
      => query
        .AddSubQuery(new IfLastUnit(IsPropertyPattern.Instance))
        .UseBuildAction(BuildStage.Create, _getPropertyAction);
  }
}
