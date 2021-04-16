using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if a building unit is an argument to inject into a property
  /// </summary>
  public record IsPropertyOfTypeMatcher : IsInjectPointOfTypeMatcher
  {
    [DebuggerStepThrough]
    public IsPropertyOfTypeMatcher(Type type, bool exactMatch) : base(type, exactMatch) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
  }
}
