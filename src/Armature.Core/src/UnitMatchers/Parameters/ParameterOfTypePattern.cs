using System;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument for a parameter of the method or the constructor requires argument of the specified type.
  /// </summary>
  public record ParameterOfTypePattern : InjectPointOfTypePattern
  {
    public ParameterOfTypePattern(Type type, bool exactMatch) : base(type, exactMatch) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
