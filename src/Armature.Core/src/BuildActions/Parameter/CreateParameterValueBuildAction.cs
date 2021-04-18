using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   "Builds" value to inject by using <see cref="ParameterInfo.ParameterType" /> and provided key
  /// </summary>
  public class CreateParameterValueBuildAction : CreateValueToInjectBuildAction
  {
    public static readonly IBuildAction Instance = new CreateParameterValueBuildAction();

    public CreateParameterValueBuildAction(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
