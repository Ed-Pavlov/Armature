using System;
using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a building unit is a generic type and it's <see cref="Type.GetGenericTypeDefinition"/> is as specified open generic type
  /// </summary>
  public record IsGenericOfDefinition : TypePatternBase, IUnitPattern
  {
    [DebuggerStepThrough]
    public IsGenericOfDefinition(Type genericTypeDefinition, object? key) : base(genericTypeDefinition, key)
    {
      if(!genericTypeDefinition.IsGenericTypeDefinition) throw new ArgumentException("Should be an open generic type", nameof(genericTypeDefinition));
    }

    public bool Matches(UnitId unitId)
    {
      var unitType = unitId.GetUnitTypeSafe();
      return Key.Matches(unitId.Key) && unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == Type;
    }
  }
}