using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the property using <see cref="PropertyInfo.PropertyType"/> and specified key as <see cref="UnitId"/>.
  /// </summary>
  public class BuildArgumentPropertyType : BuildArgumentBase
  {
    [DebuggerStepThrough]
    public BuildArgumentPropertyType(object? key = null) : base(key) { }

    protected override Type GetValueType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
