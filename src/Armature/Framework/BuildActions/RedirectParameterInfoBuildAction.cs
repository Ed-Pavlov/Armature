using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class RedirectParameterInfoBuildAction : IBuildAction
  {
    private readonly object _token;

    [DebuggerStepThrough]
    public RedirectParameterInfoBuildAction(object token = null) => _token = token;

    public void Process(IBuildSession buildSession)
    {
      var parameterInfo = (ParameterInfo)buildSession.GetUnitUnderConstruction().Id;
      var unitInfo = new UnitInfo(parameterInfo.ParameterType, _token);
      Log.Verbose("{0}: {1}", GetType().Name, unitInfo);
      buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }
  }
}