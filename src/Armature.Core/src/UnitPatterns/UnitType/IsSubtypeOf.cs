using System;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is a subtype of the specified base type.
  /// </summary>
  public record IsSubtypeOf : TypePatternBase, IUnitPattern
  {
    public IsSubtypeOf(Type baseType, object? key) : base(baseType, key) { }

    public bool Matches(UnitId unitId) => Key.Matches(unitId.Key) && Type.IsAssignableFrom(unitId.GetUnitTypeSafe());

    public override string ToString()    => base.ToString();
    public          string ToLogString() => GetType().GetShortName();
  }
}
