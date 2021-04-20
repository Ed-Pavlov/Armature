using System;
using Armature.Core;

namespace Armature
{
  public class ArgumentForPropertyTuner : LastUnitTuner, IPropertyId
  {
    private readonly IBuildAction _getPropertyAction;

    public ArgumentForPropertyTuner(IUnitPattern unitIsProperty, IBuildAction getPropertyAction, IBuildAction buildArgument, int weight)
      : base(unitIsProperty, buildArgument, weight)
      => _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    /// <summary>
    ///   In addition to the base logic adds a logic which provides a properties to inject into
    /// </summary>
    protected override void Apply(IPatternTreeNode patternTreeNode)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
        .UseBuildAction(_getPropertyAction, BuildStage.Create);
  }
}
