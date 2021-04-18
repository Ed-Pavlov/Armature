using System;

namespace Armature.Core
{
  /// <summary>
  /// Matches if a building unit's kind is a type which is a subtype of the specified base type
  /// </summary>
  public record IsSubtypePattern : UnitIdByTypeMatcherBase, IUnitIdPattern
  {
    public IsSubtypePattern(Type baseType, object? key) : base(baseType, key) { }

    public bool Matches(UnitId unitId) => Key.Matches(unitId.Key) && Type.IsAssignableFrom(unitId.GetUnitTypeSafe());
  }
}
