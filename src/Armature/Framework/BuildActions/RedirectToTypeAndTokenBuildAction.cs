using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Logging;
using Armature.Properties;

namespace Armature.Framework.BuildActions
{
  public abstract class RedirectToTypeAndTokenBuildAction : IBuildAction
  {
    private readonly object _token;

    [DebuggerStepThrough]
    protected RedirectToTypeAndTokenBuildAction([CanBeNull] object token) => _token = token;

    protected abstract Type GetValueType(UnitInfo unitInfo);
    
    public void Process(IBuildSession buildSession)
    {
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();
      var valueType = GetValueType(unitUnderConstruction);
      buildSession.BuildResult = buildSession.BuildUnit(new UnitInfo(valueType, _token));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _token.AsLogString());
  }
}