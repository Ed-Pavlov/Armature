using System;
using Armature.Core;
using Armature.Core.Logging;
using Armature.Extensibility;


namespace Armature
{
  /// <summary>
  ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding point ids
  /// </summary>
  public class InjectPropertyByInjectPointIdBuildPlan : IPropertyId, IExtensibility<object[]>
  {
    public InjectPropertyByInjectPointIdBuildPlan(params object[] pointIds) => Item1 = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public object[] Item1 { get; }

    public void Apply(IPatternTreeNode patternTreeNode)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
        .UseBuildAction(new GetPropertyListByInjectPointId(Item1), BuildStage.Create);

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", Item1));
  }
}
