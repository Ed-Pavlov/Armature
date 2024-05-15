using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit is an argument for a constructor/method parameter.
/// </summary>
public record IsParameterInfo : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Argument && unitId.Kind is ParameterInfo;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsParameterInfo);
}
