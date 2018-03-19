using System;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.BuildActions.Property;
using Armature.Framework.UnitMatchers.Properties;
using Armature.Framework.UnitSequenceMatcher;
using Armature.Logging;
using Armature.Properties;

namespace Armature
{
  public class InjectPropertyByInjectPointIdBuildPlan : IPropertyValueBuildPlan
  {
    private readonly object[] _pointIds;

    public InjectPropertyByInjectPointIdBuildPlan([NotNull] params object[] pointIds) => _pointIds = pointIds ?? throw new ArgumentNullException(nameof(pointIds));

    public void Register(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance, 0))
        .AddBuildAction(BuildStage.Create, new GetPropertyByInjectPointId(_pointIds));
    
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _pointIds));
  }
}