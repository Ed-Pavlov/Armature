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

    public void Execute(Build.Session buildSession)
    {
      Log.Verbose("{0}.Execute: Instance={1}", typeof(SingletonBuildAction).Name, _instance ?? (object)"null");

      if (_instance != null)
        buildSession.BuildResult = new BuildResult(_instance.Value);
    }

    public void PostProcess(Build.Session buildSession)
    {
      Log.Verbose("{0}.PostProcess: BuildResult={1}, Instance={2}", typeof(SingletonBuildAction).Name, buildSession.BuildResult ?? (object)"null", _instance ?? (object)"null");

      if(buildSession.BuildResult != null)
        _instance = new Instance(buildSession.BuildResult.Value);
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