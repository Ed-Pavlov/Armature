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
        .SingleOrDefault();

      if (attribute == null)
        Log.Info("{0}{{{1}}}", this, "No parameter marked with InjectAttribute");
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