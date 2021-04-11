using System;

namespace Armature.Core.UnitMatchers.UnitType
{
  public record UnitIsSubTypeOfMatcher : UnitByTypeMatcherBase, IUnitIdMatcher
  {
    public UnitIsSubTypeOfMatcher(Type baseType, object? key) : base(baseType, key) { }

    public bool Matches(UnitId unitId) => UnitType.IsAssignableFrom(unitId.GetUnitTypeSafe()) && Key.Matches(unitId.Key);
  }
}
