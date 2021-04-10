using System;
using System.Reflection;

namespace Armature.Core.BuildActions.Property
{
  public class CreatePropertyMultiValueToInjectBuildAction : CreateMultiValueToInjectBuildAction
  {
    public CreatePropertyMultiValueToInjectBuildAction(object? token = null) : base(token) { }

    protected override Type GetValueType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
