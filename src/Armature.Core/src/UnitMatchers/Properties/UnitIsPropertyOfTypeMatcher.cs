using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  ///   Matches property by exact type matching
  /// </summary>
  public record UnitIsPropertyOfTypeMatcher : UnitIsInjectPointOfTypeMatcher
  {
    [DebuggerStepThrough]
    public UnitIsPropertyOfTypeMatcher(Type propertyType) : base(propertyType) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
  }
}
