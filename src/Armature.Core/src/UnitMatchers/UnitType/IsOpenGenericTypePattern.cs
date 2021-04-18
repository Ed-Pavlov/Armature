using System;
using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  /// Matches <see cref="UnitId" /> with an open generic type
  /// </summary>
  public record IsOpenGenericTypePattern : UnitIdByTypeMatcherBase, IUnitIdPattern
  {
    [DebuggerStepThrough]
    public IsOpenGenericTypePattern(Type openType, object? key) : base(openType, key)
    {
      if(!openType.IsGenericTypeDefinition) throw new ArgumentException("Provide open generic type", nameof(openType));
    }

    public bool Matches(UnitId unitId)
    {
      var unitType = unitId.GetUnitTypeSafe();
      return Key.Matches(unitId.Key) && unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == Type;
    }
  }
}
