using System;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a building unit kind is a type and if yes can substitute the specified type.
  /// See <see cref="Type.IsAssignableFrom"/> documentation for details.
  /// </summary>
  public record IsAssignableFromType(Type Type, object? Key = null) : TypePatternBase(Type, Key), IUnitPattern
  {
    public bool Matches(UnitId unitId) => Key.Matches(unitId.Key) && unitId.GetUnitTypeSafe()?.IsAssignableFrom(Type) == true;
  }
}