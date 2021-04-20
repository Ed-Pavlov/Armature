using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Logging;
using Armature.Extensibility;


namespace Armature
{
  /// <summary>
  ///   Adds a plan injecting dependencies into properties with corresponding names
  /// </summary>
  public class InjectPropertyByNameBuildPlan : IPropertyId, IExtensibility<string[]>
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
    public void Apply(IPatternTreeNode patternTreeNode)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
        .UseBuildAction(BuildStage.Create, new GetPropertyByNameBuildAction(_names));

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}
