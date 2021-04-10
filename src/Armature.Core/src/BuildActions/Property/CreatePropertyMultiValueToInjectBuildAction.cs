using System;
using System.Reflection;

namespace Armature.Core.BuildActions.Property
{
  public class CreatePropertyMultiValueToInjectBuildAction : CreateMultiValueToInjectBuildAction
  {
    public CreatePropertyMultiValueToInjectBuildAction(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
