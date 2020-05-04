using System;
using JetBrains.Annotations;

namespace Armature.Core.UnitMatchers
{
  public class BaseTypeMatcher : UnitInfoMatcher
  {
    private readonly Type _type;
    public BaseTypeMatcher([NotNull] UnitInfo unitInfo) : base(unitInfo) => _type = unitInfo.GetUnitType();

    public override bool Matches(UnitInfo unitInfo) => _type.IsAssignableFrom(unitInfo.GetUnitTypeSafe());
  }
}