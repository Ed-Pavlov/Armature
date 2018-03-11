using System.Diagnostics;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework.BuildActions
{
  /// <summary>
  ///   Build action which caches built unit in <see cref="PostProcess" /> and then set it as
  ///   <see cref="T:Armature.Core.BuildResult" /> in <see cref="Process" />.
  ///   Simplest eternal singleton
  /// </summary>
  public class SingletonBuildAction : IBuildAction
  {
    private Instance _instance;

    [DebuggerStepThrough]
    public SingletonBuildAction() { }

    [DebuggerStepThrough]
    public SingletonBuildAction([CanBeNull] object value) => _instance = new Instance(value);

    public void Process(IBuildSession buildSession)
    {
      if (_instance != null)
        buildSession.BuildResult = new BuildResult(_instance.Value);
    }

    public void PostProcess(IBuildSession buildSession)
    {
      if (buildSession.BuildResult != null)
        _instance = new Instance(buildSession.BuildResult.Value);
    }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}: {1}", GetType().Name, _instance ?? (object)"not set");

    private class Instance
    {
      public readonly object Value;

      [DebuggerStepThrough]
      public Instance([CanBeNull] object value) => Value = value;

      [DebuggerStepThrough]
      public override string ToString() => Value == null ? "[no Instance]" : "Instance=" + Value;
    }
  }
}