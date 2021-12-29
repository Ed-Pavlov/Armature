using System;
using System.Reflection;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the property using <see cref="PropertyInfo.PropertyType"/> and specified tag as <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByPropertyType : BuildArgumentByInjectPointTypeBase
{
  public BuildArgumentByPropertyType() { }
  public BuildArgumentByPropertyType(object? tag = null) : base(tag) { }

  protected override Type GetInjectPointType(UnitId unitId) => ((PropertyInfo) unitId.Kind!).PropertyType;
}
