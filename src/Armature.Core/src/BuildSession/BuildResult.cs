using System.Diagnostics;

namespace Armature.Core;

/// <summary>
/// A result of building a Unit, null is a valid value of the <see cref="Value" />.
/// </summary>
[method: DebuggerStepThrough]
public readonly struct BuildResult(object? value)
{
  public readonly object? Value    = value;
  public readonly bool    HasValue = true;

  [DebuggerStepThrough]
  public override string ToString() => HasValue ? Value.ToHoconString() : nameof(BuildResult) + ".Nothing";
}
