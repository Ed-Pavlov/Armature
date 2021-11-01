using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the property using <see cref="PropertyInfo.PropertyType"/> and specified key as <see cref="UnitId"/>.
  /// </summary>
  public record BuildArgumentByPropertyType : BuildArgumentByInjectPointTypeBase
  {
    public BuildArgumentByPropertyType() { }
    public BuildArgumentByPropertyType(object? key = null) : base(key) { }

    protected override Type GetInjectPointType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
  }
}