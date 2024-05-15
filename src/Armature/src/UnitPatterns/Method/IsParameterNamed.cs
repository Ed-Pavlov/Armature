using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit is an argument for a constructor/method parameter with the specified name.
/// </summary>
public record IsParameterNamed : InjectPointNamedBase
{
  [DebuggerStepThrough]
  public IsParameterNamed(string name) : base(name) { }

  protected override string? GetInjectPointName(UnitId unitId) => (unitId.Kind as ParameterInfo)?.Name;
}