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
  public class PropertyListByName : LastUnitTuner, IInjectPointTuner //, IExtensibility<string[]>
  {
    private readonly string[] _names;

    [DebuggerStepThrough]
    public PropertyListByName(string[] names, int weight) : base(PropertiesListPattern.Instance, new GetPropertyListByNameBuildAction(names), weight)
    {
      if(names is null || names.Length == 0) throw new ArgumentNullException(nameof(names));

      _names = names;
    }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), string.Join(", ", _names));
  }
}
