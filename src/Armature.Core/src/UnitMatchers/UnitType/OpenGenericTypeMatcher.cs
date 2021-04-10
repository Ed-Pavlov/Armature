using System;
using System.Diagnostics;

namespace Armature.Core.UnitMatchers.UnitType
{
  /// <summary>
  ///   Matches <see cref="UnitId" /> with an open generic type
  /// </summary>
  public record OpenGenericTypeMatcher : UnitInfoByTypeMatcherBase, IUnitIdMatcher
  {
    [DebuggerStepThrough]
    public OpenGenericTypeMatcher(Type type, object? key) : base(type, key)
    {
      if(!type.IsGenericTypeDefinition) throw new ArgumentException("Provide open generic type", nameof(type));
    }

    public bool Matches(UnitId unitId)
    {
      var unitType = unitId.GetUnitTypeSafe();
      return unitType is {IsGenericType: true} && unitType.GetGenericTypeDefinition() == UnitType && Key.Matches(unitId.Key);
    }
  }
}
