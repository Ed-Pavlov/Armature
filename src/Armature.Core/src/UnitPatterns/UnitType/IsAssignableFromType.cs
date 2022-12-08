using System;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks whether an instance of a specified type can be assigned to an instance of the type represented by <see cref="UnitId.Kind"/>
/// See <see cref="Type.IsAssignableFrom"/> documentation for details.
/// </summary>
public record IsAssignableFromType(Type Type, object? Tag = null) : TypePatternBase(Type, Tag), IUnitPattern
{
  public bool Matches(UnitId unitId) => Tag.Matches(unitId.Tag) && unitId.GetUnitTypeSafe()?.IsAssignableFrom(Type) == true;
}
