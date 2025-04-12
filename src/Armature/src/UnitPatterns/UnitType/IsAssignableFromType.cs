using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks whether an instance of a specified type can be assigned to an instance of the type represented by <see cref="UnitId.Kind"/>
/// See <see cref="Type.IsAssignableFrom"/> documentation for details.
/// </summary>
public sealed record IsAssignableFromType(Type Type, object? Tag = null) : TypePatternBase(Type, Tag), IUnitPattern
{
  public bool Matches(UnitId unitId) => _tag.Matches(unitId.Tag) && unitId.GetUnitTypeSafe()?.IsAssignableFrom(_type) == true;
}
