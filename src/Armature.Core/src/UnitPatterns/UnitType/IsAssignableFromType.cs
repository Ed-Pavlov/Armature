using System;
using Armature.Core.Sdk;

namespace Armature.Core
{
  /// <summary>
  /// Determines whether an instance of a specified type can be assigned to an instance of the type represented by <see cref="UnitId.Kind"/>
  /// See <see cref="Type.IsAssignableFrom"/> documentation for details.
  /// </summary>
  public record IsAssignableFromType(Type Type, object? Key = null) : TypePatternBase(Type, Key), IUnitPattern
  {
    public bool Matches(UnitId unitId) => Key.Matches(unitId.Key) && unitId.GetUnitTypeSafe()?.IsAssignableFrom(Type) == true;
  }
}