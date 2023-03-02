using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Sdk;

namespace Armature.UnitPatterns.Property;

/// <summary>
/// Checks if a unit is an argument to inject into the property.
/// </summary>
public record IsPropertyInfo : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Argument && unitId.Kind is PropertyInfo;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsPropertyInfo);
}
