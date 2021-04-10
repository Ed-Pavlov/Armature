using System;


namespace Armature.Core.UnitMatchers
{
  public record BaseTypeMatcher : UnitInfoMatcher
  {
    private readonly Type _type;
    public BaseTypeMatcher(UnitInfo unitInfo) : base(unitInfo) => _type = unitInfo.GetUnitType();

    public override bool Matches(UnitInfo unitInfo) => _type.IsAssignableFrom(unitInfo.GetUnitTypeSafe());
  }
}
