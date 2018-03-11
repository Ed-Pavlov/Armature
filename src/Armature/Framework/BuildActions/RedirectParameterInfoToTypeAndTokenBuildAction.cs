using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class RedirectParameterInfoToTypeAndTokenBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new RedirectParameterInfoToTypeAndTokenBuildAction();
    
    public void Process(IBuildSession buildSession)
    {
      var parameterInfo = (ParameterInfo)buildSession.GetUnitUnderConstruction().Id;

      var attribute = parameterInfo
        .GetCustomAttributes(typeof(InjectAttribute), true)
        .OfType<InjectAttribute>()
        .Single();

      var unitInfo = new UnitInfo(parameterInfo.ParameterType, attribute.InjectionPointId);
      Log.Verbose("{0}: {1}", GetType().Name, unitInfo);
      buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
    }

    public void PostProcess(IBuildSession buildSession) { }
  }
}