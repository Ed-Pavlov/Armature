using System.Diagnostics;

namespace Armature.Core
{
  /// <summary>
  ///   Injects values into building Unit properties specified for injection
  /// </summary>
  public class InjectIntoProperties : IBuildAction
  {
    public static readonly IBuildAction Instance = new InjectIntoProperties();

    private InjectIntoProperties() { }

    [DebuggerStepThrough]
    public void Process(IBuildSession buildSession) { }

    public void PostProcess(IBuildSession buildSession)
    {
      if(buildSession.BuildResult.HasValue)
      {
        var unit       = buildSession.BuildResult.Value;
        var type       = unit!.GetType();
        var properties = buildSession.GetPropertiesToInject(type);

        foreach(var property in properties)
        {
          var value = buildSession.GetValueForProperty(property);
          property.SetValue(unit, value);
        }
      }
    }
  }
}
