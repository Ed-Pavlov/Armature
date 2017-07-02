using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// Build action which caches built unit in <see cref="PostProcess"/> and then set it as <see cref="BuildResult"/> in <see cref="Process"/>.
  /// Simplest eternal singleton 
  /// </summary>
  public class SingletonBuildAction : IBuildAction
  {
    private Instance _instance;

    public SingletonBuildAction()
    {}

    public SingletonBuildAction([CanBeNull] object value)
    {
      _instance = new Instance(value);
    }

    public void Process(UnitBuilder unitBuilder)
    {
      if (_instance != null)
        unitBuilder.BuildResult = new BuildResult(_instance.Value);
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {
      if(unitBuilder.BuildResult != null)
        _instance = new Instance(unitBuilder.BuildResult.Value);
    }

    public override string ToString()
    {
      return string.Format("{0}: {1}", GetType().Name, _instance ?? (object)"not set");
    }

    private class Instance
    {
      public readonly object Value;

      public Instance([CanBeNull] object value)
      {
        Value = value;
      }

      public override string ToString()
      {
        return Value == null ? "[no Instance]" : "Instance=" + Value;
      }
    }
  }
}