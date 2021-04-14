using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions.Property;
using Armature.Core.Logging;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;


namespace Armature
{
  /// <summary>
  ///   Adds a plan injecting dependencies into properties with corresponding names
  /// </summary>
  public class InjectPropertyByNameBuildPlan : IPropertyValueBuildPlan, IExtensibility<string[]>
  {
    private readonly string[] _names;

    [DebuggerStepThrough]
    public InjectPropertyByNameBuildPlan(params string[] names)
    {
      if(names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));

      _names = names;
    }

    string[] IExtensibility<string[]>.Item1 => _names;

    [DebuggerStepThrough]
    public void Apply(IScannerTree scannerTree)
      => scannerTree
        .AddItem(new IfLastUnitIs(UnitIsPropertyMatcher.Instance))
        .AddBuildAction(BuildStage.Create, new GetPropertyByNameBuildAction(_names));

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}
