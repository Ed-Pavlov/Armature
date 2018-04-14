using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Armature.Logging;
using Resharper.Annotations;

namespace Armature
{
  public class InjectPropertyByNameBuildPlan : IPropertyValueBuildPlan
  {
    private readonly string[] _names;

    [DebuggerStepThrough]
    public InjectPropertyByNameBuildPlan([NotNull] params string[] names)
    {
      if (names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));

      _names = names;
    }

    [DebuggerStepThrough]
    public void Register(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetPropertyByNameBuildAction(_names));
    
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}