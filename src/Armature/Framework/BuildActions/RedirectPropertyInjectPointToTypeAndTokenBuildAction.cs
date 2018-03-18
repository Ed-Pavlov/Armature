using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class RedirectPropertyInjectPointToTypeAndTokenBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new RedirectPropertyInjectPointToTypeAndTokenBuildAction();

    private RedirectPropertyInjectPointToTypeAndTokenBuildAction()
    {
    }

    public void Process(IBuildSession buildSession)
    {
      var propertyInfo = (PropertyInfo)buildSession.GetUnitUnderConstruction().Id;

      var attribute = propertyInfo
        .GetCustomAttributes<InjectAttribute>()
        .SingleOrDefault();

      if (attribute == null)
        Log.Info("{0}{{{1}}}", this, "No Property marked with InjectAttribute");
      else
      {
        var unitInfo = new UnitInfo(propertyInfo.PropertyType, attribute.InjectionPointId);
        buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
      }
    }

    public void PostProcess(IBuildSession buildSession) {  }
  }
}