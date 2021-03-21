using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core.BuildActions
{
  /// <summary>
  ///   Build action which caches built unit in <see cref="PostProcess" /> and then set it as
  ///   <see cref="BuildResult" /> in <see cref="Process" />.
  ///   Simplest eternal singleton.
  /// </summary>
  public class SingletonBuildAction : IBuildAction
  {
    private BuildResult _instance;

    [DebuggerStepThrough]
    public SingletonBuildAction() { }

    [DebuggerStepThrough]
    public SingletonBuildAction([CanBeNull] object value) => _instance = new BuildResult(value);

    public void Process(IBuildSession buildSession)
    {
      if (_instance is not null) 
        buildSession.BuildResult = _instance;
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if (buildSession.BuildResult is not null)
        _instance = buildSession.BuildResult; //TODO: can this code hold some references and memory?
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().Name, _instance);
  }
}