using System;
using System.Reflection;

namespace Armature.Core
{
  public class CreateParameterMultiValueToInjectBuildAction : CreateMultiValueToInjectBuildAction
  {
    public static readonly CreateParameterMultiValueToInjectBuildAction Instance = new();

    public CreateParameterMultiValueToInjectBuildAction(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
  }
}
