using System;

namespace Armature.Core
{
  public record IsAssignableFromMatcher : UnitIdByTypeMatcherBase, IUnitIdMatcher
  {
    public IsAssignableFromMatcher(Type baseType, object? key) : base(baseType, key) { }

    public bool Matches(UnitId unitId) => Key.Matches(unitId.Key) && Type.IsAssignableFrom(unitId.GetUnitTypeSafe());
  }
}
