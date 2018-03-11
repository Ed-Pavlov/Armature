using System.Diagnostics;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  ///   Build action which caches built unit in <see cref="M:Armature.Framework.SingletonBuildAction.PostProcess(Armature.Core.UnitBuilder)" /> and then set it as
  ///   <see cref="T:Armature.Core.BuildResult" /> in <see cref="M:Armature.Framework.SingletonBuildAction.Process(Armature.Core.UnitBuilder)" />.
  ///   Simplest eternal singleton
  /// </summary>
  public class SingletonBuildAction : IBuildAction
  {
    private Instance _instance;

    [DebuggerStepThrough]
    public SingletonBuildAction() { }

    [DebuggerStepThrough]
    public SingletonBuildAction([CanBeNull] object value) => _instance = new Instance(value);

    public void Process(UnitBuilder unitBuilder)
    {
      if (_instance != null)
        unitBuilder.BuildResult = new BuildResult(_instance.Value);
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {
      if (unitBuilder.BuildResult != null)
        _instance = new Instance(unitBuilder.BuildResult.Value);
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