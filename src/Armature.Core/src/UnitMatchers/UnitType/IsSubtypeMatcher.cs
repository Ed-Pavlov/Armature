using System;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit's kind is a type which is a subtype of the specified base type
  /// </summary>
  public record IsSubtypeMatcher : UnitIdByTypeMatcherBase, IUnitIdMatcher
  {
    public IsSubtypeMatcher(Type baseType, object? key) : base(baseType, key) { }

    public bool Matches(UnitId unitId) => Key.Matches(unitId.Key) && Type.IsAssignableFrom(unitId.GetUnitTypeSafe());
  }
}
