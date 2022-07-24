using System;
using System.Reflection;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is an argument for a parameter of the method or the constructor requires argument of the specified type.
/// </summary>
public record IsMethodParameterOfType : InjectPointOfTypePatternBase
{
  public IsMethodParameterOfType(IUnitPattern typePattern) : base(typePattern) { }

  protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
}