using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the property using <see cref="PropertyInfo.PropertyType"/> and specified key as <see cref="UnitId"/>.
  /// </summary>
  public class BuildArgumentByPropertyType : BuildArgumentByInjectPointTypeBase
  {
    [DebuggerStepThrough]
    public BuildArgumentByPropertyType(object? key = null) : base(key) { }

    protected override Type GetInjectPointType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}
