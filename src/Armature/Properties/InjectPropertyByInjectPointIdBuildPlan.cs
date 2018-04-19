using System;
using Armature.Core;
using Armature.Core.BuildActions.Property;
using Armature.Core.Logging;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  /// <summary>
  /// Adds a plan injecting dependencies into properties marked with <see cref="InjectAttribute"/> with corresponding point ids
  /// </summary>
  public class InjectPropertyByInjectPointIdBuildPlan : IPropertyValueBuildPlan
  {
    private readonly object[] _pointIds;

    public InjectPropertyByInjectPointIdBuildPlan([NotNull] params object[] pointIds) => _pointIds = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public void Apply(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetPropertyByInjectPointBuildAction(_pointIds));
    
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _pointIds));
  }
}