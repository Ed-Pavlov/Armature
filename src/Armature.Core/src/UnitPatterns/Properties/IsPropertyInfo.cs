using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;
using Armature.Core.Sdk;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit is an argument to inject into the property.
  /// </summary>
  public record IsPropertyInfo : IUnitPattern
  {
    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Argument && unitId.Kind is PropertyInfo;

    [DebuggerStepThrough]
    public override string ToString() => nameof(IsPropertyInfo);
  }
}