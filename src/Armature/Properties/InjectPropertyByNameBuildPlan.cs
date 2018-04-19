using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions.Property;
using Armature.Core.Logging;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  /// <summary>
  /// Adds a plan injecting dependencies into properties with corresponding names
  /// </summary>
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
    public void Apply(IUnitSequenceMatcher unitSequenceMatcher) =>
      unitSequenceMatcher
        .AddOrGetUnitSequenceMatcher(new LastUnitSequenceMatcher(PropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetPropertyByNameBuildAction(_names));
    
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}