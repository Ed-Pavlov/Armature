using System;
using Armature.Core;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Armature.Logging;
using Resharper.Annotations;

namespace Armature
{
  public class InjectPropertyByInjectPointIdBuildPlan : IPropertyValueBuildPlan
  {
    private readonly object[] _pointIds;

    public InjectPropertyByInjectPointIdBuildPlan([NotNull] params object[] pointIds) => _pointIds = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public void Register(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetPropertyByInjectPointBuildAction(_pointIds));
    
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _pointIds));
  }
}