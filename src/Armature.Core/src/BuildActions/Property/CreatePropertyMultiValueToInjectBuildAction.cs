using System;
using System.Reflection;

namespace Armature.Core.BuildActions.Property
{
  public class CreatePropertyMultiValueToInjectBuildAction : CreateMultiValueToInjectBuildAction
  {
    public CreatePropertyMultiValueToInjectBuildAction(object? token = null) : base(token) { }

    protected override Type GetValueType(UnitInfo unitInfo) => ((PropertyInfo) unitInfo.Id!).PropertyType;
  }
}
