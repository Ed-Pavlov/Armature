using System.Linq;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Builds an argument for the method parameter marked with <see cref="InjectAttribute"/> using <see cref="InjectAttribute.InjectionPointId"/> as
  ///   the <see cref="UnitId.Key"/>
  /// </summary>
  public class BuildArgumentForMethodWithPointIdAsKey : IBuildAction
  {
    public static readonly IBuildAction Instance = new BuildArgumentForMethodWithPointIdAsKey();

    private BuildArgumentForMethodWithPointIdAsKey() { }

    public void Process(IBuildSession buildSession)
    {
      var parameterInfo = (ParameterInfo) buildSession.GetUnitUnderConstruction().Kind!;

      var attribute = parameterInfo
                     .GetCustomAttributes<InjectAttribute>()
                     .SingleOrDefault();

      if(attribute is null)
      {
        Log.WriteLine(LogLevel.Verbose, () => $"{this} => parameter is not marked with InjectAttribute");
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
