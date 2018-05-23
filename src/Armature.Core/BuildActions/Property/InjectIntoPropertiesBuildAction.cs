using System.Diagnostics;

namespace Armature.Core.BuildActions.Property
{
  /// <summary>
  ///   Injects values into building Unit properties specified for injection
  /// </summary>
  public class InjectIntoPropertiesBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new InjectIntoPropertiesBuildAction();

    private InjectIntoPropertiesBuildAction() { }

    [DebuggerStepThrough]
    public void Process(IBuildSession buildSession) { }

    public void PostProcess(IBuildSession buildSession)
    {
      var unit = buildSession.BuildResult?.Value;
      if (unit != null)
      {
        var type = unit.GetType();
        var properties = buildSession.GetPropertiesToInject(type);

        foreach (var property in properties)
        {
          var value = buildSession.GetValueForProperty(property);
          property.SetValue(unit, value);
        }
      }
    }
  }
}