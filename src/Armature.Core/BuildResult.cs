using System.Diagnostics;
using Armature.Core.Logging;
using Resharper.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Represents a result of building of an Unit, null is a valid value of the <see cref="Value" />.
  /// </summary>
  public struct BuildResult
  {
    [CanBeNull]
    public readonly object Value;

    public readonly bool HasValue;

    [DebuggerStepThrough]
    public BuildResult([CanBeNull] object value)
    {
      HasValue = true;
      Value = value;
    }

    [DebuggerStepThrough]
    public override string ToString() => Value.AsLogString();
  }
}