using System;
using System.Reflection;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is an argument for a constructor/method parameter of the specified type.
/// </summary>
public record IsParameterOfType : InjectPointOfTypeBase
{
  public IsParameterOfType(IUnitPattern typePattern) : base(typePattern) { }

  protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
}