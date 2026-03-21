using System.Diagnostics;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Thread-safe eternal singleton.
/// Caches just built Unit in <see cref="PostProcess" /> and then set it as <see cref="BuildResult" /> in <see cref="Process" />.
/// </summary>
public sealed record ThreadSafeSingleton : IBuildAction, ILogString
{
  private readonly object _lock = new();

  private bool    _hasInstance;
  private object? _instance;

  public void Process(IBuildSession buildSession)
  {
    lock(_lock)
      if(_hasInstance)
        buildSession.BuildResult = new BuildResult(_instance);
  }

  public void PostProcess(IBuildSession buildSession)
  {
    lock(_lock)
      if(buildSession.BuildResult.HasValue)
      {
        _instance    = buildSession.BuildResult.Value;
        _hasInstance = true;
      }
  }

  [DebuggerStepThrough]
  public string ToHoconString()
  {
    lock(_lock)
      return Hocon.Object<ThreadSafeSingleton>(("instance", _hasInstance ? _instance : "nothing"));
  }
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
