using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit is a "service" unit, constructor, argument, etc.
/// </summary>
[PublicAPI]
public record IsServiceUnit : IUnitPattern, ILogString
{
  public virtual bool Matches(UnitId unitId) => unitId.Tag is ServiceTag;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsServiceUnit);

  [DebuggerStepThrough]
  public string ToHoconString() => ToString();
}