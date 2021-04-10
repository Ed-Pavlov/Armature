using System;

namespace Armature.Core.UnitMatchers.UnitType
{
  public record BaseTypeMatcher : UnitInfoByTypeMatcherBase, IUnitMatcher
  {
    public BaseTypeMatcher(Type baseType, object? token) : base(baseType, token) { }

    public bool Matches(UnitId unitId) => UnitType.IsAssignableFrom(unitId.GetUnitTypeSafe()) && Token.Matches(unitId.Key);
  }
}
