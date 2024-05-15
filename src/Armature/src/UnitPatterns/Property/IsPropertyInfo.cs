using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit is an argument to inject into the property.
/// </summary>
public record IsPropertyInfo : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Argument && unitId.Kind is PropertyInfo;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsPropertyInfo);
}
