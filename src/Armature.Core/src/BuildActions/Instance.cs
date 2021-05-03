using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Caches passed value and set it as <see cref="BuildResult" /> in <see cref="Process" />.
  ///   Simplest eternal singleton.
  /// </summary>
  public class Instance<T> : IBuildAction
  {
    private readonly T? _instance;

    [DebuggerStepThrough]
    public Instance(T? value) => _instance = value;

    public void Process(IBuildSession buildSession) => buildSession.BuildResult = new BuildResult(_instance);

    public void PostProcess(IBuildSession buildSession)
    {
    }

    [DebuggerStepThrough]
    public override string ToString() => $"{GetType().Name}( {(_instance.ToLogString())} )";
  }
}
