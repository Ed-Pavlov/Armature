using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class SingletonBuildAction : IBuildAction
  {
    private Instance _instance;

    public SingletonBuildAction()
    {}

    public SingletonBuildAction([CanBeNull] object value)
    {
      _instance = new Instance(value);
    }

    public void Execute(UnitBuilder unitBuilder)
    {
      Log.Verbose("{0}.Execute: Instance={1}", typeof(SingletonBuildAction).Name, _instance ?? (object)"null");

      if (_instance != null)
        unitBuilder.BuildResult = new BuildResult(_instance.Value);
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {
      Log.Verbose("{0}.PostProcess: BuildResult={1}, Instance={2}", typeof(SingletonBuildAction).Name, unitBuilder.BuildResult ?? (object)"null", _instance ?? (object)"null");

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
        return Value == null ? "null" : Value.ToString();
      }
    }
  }
}