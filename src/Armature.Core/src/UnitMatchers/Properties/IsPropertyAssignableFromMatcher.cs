using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Checks if a building unit is an argument to inject into a property
  /// </summary>
  public record IsPropertyAssignableFromMatcher : IsInjectPointAssignableFromMatcher
  {
    [DebuggerStepThrough]
    public IsPropertyAssignableFromMatcher(Type type) : base(type) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
  }
}
