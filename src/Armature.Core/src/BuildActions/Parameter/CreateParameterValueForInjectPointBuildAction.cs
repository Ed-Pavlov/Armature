using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Builds value to inject by using <see cref="ParameterInfo.ParameterType" /> and <see cref="InjectAttribute.InjectionPointId" /> as key
  /// </summary>
  public class CreateParameterValueForInjectPointBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new CreateParameterValueForInjectPointBuildAction();

    private CreateParameterValueForInjectPointBuildAction() { }

    public void Process(IBuildSession buildSession)
    {
      var parameterInfo = (ParameterInfo) buildSession.GetUnitUnderConstruction().Kind!;

      var attribute = parameterInfo
                     .GetCustomAttributes<InjectAttribute>()
                     .SingleOrDefault();

      if(attribute is null)
      {
        Log.WriteLine(LogLevel.Info, () => string.Format("{0}{{{1}}}", this, "No parameter marked with InjectAttribute"));
      }
      else
      {
        var unitInfo = new UnitId(parameterInfo.ParameterType, attribute.InjectionPointId);
        buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
      }
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}
