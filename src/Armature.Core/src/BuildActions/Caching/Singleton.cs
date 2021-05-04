using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Caches built unit in <see cref="PostProcess" /> and then set it as <see cref="BuildResult" /> in <see cref="Process" />.
  ///   Simplest eternal singleton.
  /// </summary>
  public class Singleton : IBuildAction
  {
    private Instance? _instance;

    public void Process(IBuildSession buildSession)
    {
      if(_instance is not null)
        buildSession.BuildResult = new BuildResult(_instance.Value);
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if(buildSession.BuildResult.HasValue)
        _instance = new Instance(buildSession.BuildResult.Value);
    }

    private class Instance
    {
      public readonly object? Value;

      [DebuggerStepThrough]
      public Instance(object? value) => Value = value;

      [DebuggerStepThrough]
      public override string ToString() => Value is null ? "null" : Value.ToLogString();
    }
  }
}
