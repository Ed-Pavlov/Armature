using System;
using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a building is of an open generic type.
  /// </summary>
  public record IsOpenGenericType : TypePatternBase, IUnitPattern
  {
    [DebuggerStepThrough]
    public IsOpenGenericType(Type openType, object? key) : base(openType, key)
    {
      if(!openType.IsGenericTypeDefinition) throw new ArgumentException("Should be an open generic type", nameof(openType));
    }

    public bool Matches(UnitId unitId)
    {
      var unitType = unitId.GetUnitTypeSafe();
      return Key.Matches(unitId.Key) && unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == Type;
    }
  }
}
