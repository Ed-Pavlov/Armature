using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   Builds value to inject into property by using <see cref="PropertyInfo.PropertyType" /> and provided key
  /// </summary>
  public class CreatePropertyValueBuildAction : CreateValueToInjectBuildAction
  {
    [DebuggerStepThrough]
    public CreatePropertyValueBuildAction(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
