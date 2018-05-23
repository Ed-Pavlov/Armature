using System.Diagnostics;
using Armature.Core.Logging;
using Resharper.Annotations;

namespace Armature.Core
{
  /// <summary>
  ///   Represents a result of building of an Unit, null is a valid value of the <see cref="Value" />.
  /// </summary>
  public class BuildResult
  {
    [CanBeNull]
    public readonly object Value;

    [DebuggerStepThrough]
    public BuildResult([CanBeNull] object value) => Value = value;

    [DebuggerStepThrough]
    public override string ToString() => Value.AsLogString();
  }
}