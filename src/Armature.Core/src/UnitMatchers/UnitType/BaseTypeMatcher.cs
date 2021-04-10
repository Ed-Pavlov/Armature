using System;

namespace Armature.Core.UnitMatchers.UnitType
{
  public record BaseTypeMatcher : UnitInfoByTypeMatcherBase, IUnitMatcher
  {
    public BaseTypeMatcher(Type baseType, object? key) : base(baseType, key) { }

    public bool Matches(UnitId unitId) => UnitType.IsAssignableFrom(unitId.GetUnitTypeSafe()) && Key.Matches(unitId.Key);
  }
}
