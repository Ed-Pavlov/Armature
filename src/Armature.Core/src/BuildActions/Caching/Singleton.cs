using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// The simplest eternal singleton.
/// Caches just built Unit in <see cref="PostProcess" /> and then set it as <see cref="BuildResult" /> in <see cref="Process" />.
/// </summary>
public record Singleton : IBuildAction, ILogString
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

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(Singleton)}{{ Instance: {(_hasInstance ? _instance.ToHoconString() : "nothing")} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
