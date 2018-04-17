using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Parameter
{
  /// <summary>
  /// Builds value to inject by using <see cref="ParameterInfo.ParameterType"/> and <see cref="InjectAttribute.InjectionPointId"/> as token
  /// </summary>
  public class CreateParameterValueForInjectPointBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new CreateParameterValueForInjectPointBuildAction();

    private CreateParameterValueForInjectPointBuildAction()
    {
    }

    public void Process(IBuildSession buildSession)
    {
      var parameterInfo = (ParameterInfo)buildSession.GetUnitUnderConstruction().Id;

      var attribute = parameterInfo
        .GetCustomAttributes<InjectAttribute>()
        .SingleOrDefault();

      if (attribute == null)
        Log.WriteLine(LogLevel.Info, "{0}{{{1}}}", this, "No parameter marked with InjectAttribute");
      else
      {
        var unitInfo = new UnitInfo(parameterInfo.ParameterType, attribute.InjectionPointId);
        buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
      }
    }

    public void PostProcess(IBuildSession buildSession) { }
    
    public override string ToString() => GetType().GetShortName();
  }
}