using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is an argument to inject into a property requires argument of the specified type.
/// </summary>
public record IsPropertyOfType : InjectPointOfTypePatternBase
{
  [DebuggerStepThrough]
  public IsPropertyOfType(IUnitPattern typePattern) : base(typePattern) { }

  protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
}