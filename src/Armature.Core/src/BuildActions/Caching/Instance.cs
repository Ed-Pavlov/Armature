using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Caches passed object and set it as <see cref="BuildResult" /> in <see cref="Process" />.
  ///   Simplest eternal singleton.
  /// </summary>
  public record Instance<T> : IBuildAction
  {
    private readonly T _value;

    [DebuggerStepThrough]
    public Instance(T value) => _value = value;

    public void Process(IBuildSession buildSession) => buildSession.BuildResult = new BuildResult(_value);

    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().Name}( {(_value.ToLogString())} )";
  }
}
