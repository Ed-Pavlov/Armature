using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit to be built is a type constructor
/// </summary>
public record IsConstructor : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Constructor;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsConstructor);
}
