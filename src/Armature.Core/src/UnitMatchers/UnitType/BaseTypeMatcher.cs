using System;

namespace Armature.Core.UnitMatchers.UnitType
{
  public record BaseTypeMatcher : UnitInfoByTypeMatcherBase, IUnitMatcher
  {
    public BaseTypeMatcher(Type baseType, object? token) : base(baseType, token) {}

    public bool Matches(UnitInfo unitInfo) => UnitType.IsAssignableFrom(unitInfo.GetUnitTypeSafe()) && Token.Matches(unitInfo.Token);
  }
}
