using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument to inject into a property requires argument of the specified type.
  /// </summary>
  public record IsPropertyWithType : InjectPointByTypePattern
  {
    [DebuggerStepThrough]
    public IsPropertyWithType(Type type, bool exactMatch) : base(type, exactMatch) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
  }
}
