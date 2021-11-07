using System.Diagnostics;
using Armature.Core.Sdk;


namespace Armature.Core
{
  /// <summary>
  /// A result of building of a Unit, null is a valid value of the <see cref="Value" />.
  /// </summary>
  public readonly struct BuildResult
  {
    public readonly object? Value;

    public readonly bool HasValue;

    [DebuggerStepThrough]
    public BuildResult(object? value)
    {
      HasValue = true;
      Value    = value;
    }

    [DebuggerStepThrough]
    public override string ToString() => HasValue ? Value.ToHoconString() : "nothing";
  }
}
