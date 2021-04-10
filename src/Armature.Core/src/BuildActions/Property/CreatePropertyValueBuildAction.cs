using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   Builds value to inject into property by using <see cref="PropertyInfo.PropertyType" /> and provided token
  /// </summary>
  public class CreatePropertyValueBuildAction : CreateValueToInjectBuildAction
  {
    [DebuggerStepThrough]
    public CreatePropertyValueBuildAction(object? token = null) : base(token) { }

    protected override Type GetValueType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
