using System.Diagnostics;

namespace BeatyBit.Armature.Core;

/// <summary>
/// A result of building a Unit, null is a valid value of the <see cref="Value" />.
/// </summary>
public readonly struct BuildResult
{
  public readonly object? Value;
  public readonly bool    HasValue;

  [DebuggerStepThrough]
  public BuildResult(object? value)
  {
    Value    = value;
    HasValue = true;
  }

  [DebuggerStepThrough]
  public override string ToString() => HasValue ? Value.ToHoconString() : nameof(BuildResult) + ".Nothing";
}
