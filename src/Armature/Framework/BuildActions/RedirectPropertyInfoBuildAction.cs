using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class RedirectPropertyInfoBuildAction : IBuildAction
  {
    private readonly object _token;

    [DebuggerStepThrough]
    public RedirectPropertyInfoBuildAction(object token = null) => _token = token;

    public void Process(IBuildSession buildSession)
    {
      var propertyInfo = (PropertyInfo)buildSession.GetUnitUnderConstruction().Id;
      var unitInfo = new UnitInfo(propertyInfo.PropertyType, _token);
      buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _token.AsLogString());
  }
}