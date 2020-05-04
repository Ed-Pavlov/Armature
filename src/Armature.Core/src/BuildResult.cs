using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Represents a result of building of a Unit, null is a valid value of the <see cref="Value" />.
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
    public override string ToString() => Value.ToLogString();
  }
}