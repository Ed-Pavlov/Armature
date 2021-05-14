using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Caches built unit in <see cref="PostProcess" /> and then set it as <see cref="BuildResult" /> in <see cref="Process" />.
  ///   Simplest eternal singleton.
  /// </summary>
  public class Singleton : IBuildAction
  {
    private bool    _hasInstance;
    private object? _instance;

    public void Process(IBuildSession buildSession)
    {
      if(_hasInstance)
        buildSession.BuildResult = new BuildResult(_instance);
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if(buildSession.BuildResult.HasValue)
      {
        _instance    = buildSession.BuildResult.Value;
        _hasInstance = true;
      }
    }

    public override string ToString() => $"{typeof(Singleton)}{{ {(_hasInstance ? _instance.ToLogString() : "no instance")} }}";
  }
}
