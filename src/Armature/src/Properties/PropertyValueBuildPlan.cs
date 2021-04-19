using System;
using Armature.Core;


namespace Armature
{
  public class PropertyValueBuildPlan : LastUnitTuner, IPropertyValueBuildPlan
  {
    private readonly IBuildAction _getPropertyAction;

    public PropertyValueBuildPlan(
      IUnitPattern propertyPattern,
      IBuildAction   getPropertyAction,
      IBuildAction   buildArgument,
      int            weight)
      : base(propertyPattern, buildArgument, weight)
      => _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    /// <summary>
    ///   In addition to the base logic adds a logic which provides a properties to inject into
    /// </summary>
    /// <param name="patternTreeNode"></param>
    protected override void Apply(IPatternTreeNode patternTreeNode)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
        .UseBuildAction(BuildStage.Create, _getPropertyAction);
  }
}
