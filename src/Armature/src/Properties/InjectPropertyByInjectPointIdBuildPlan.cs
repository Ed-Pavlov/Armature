using System;
using Armature.Core;
using Armature.Core.BuildActions.Property;
using Armature.Core.Logging;
using Armature.Extensibility;


namespace Armature
{
  /// <summary>
  ///   Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute" /> with corresponding point ids
  /// </summary>
  public class InjectPropertyByInjectPointIdBuildPlan : IPropertyValueBuildPlan, IExtensibility<object[]>
  {
    public InjectPropertyByInjectPointIdBuildPlan(params object[] pointIds) => Item1 = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public object[] Item1 { get; }

    public void Apply(IQuery query)
      => query
        .AddSubQuery(new IfLastUnitIs(UnitIsPropertyMatcher.Instance))
        .UseBuildAction(BuildStage.Create, new GetPropertyByInjectPointBuildAction(Item1));

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", Item1));
  }
}
