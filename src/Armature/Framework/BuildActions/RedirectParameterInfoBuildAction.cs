using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.BuildActions
{
  public class RedirectParameterInfoBuildAction : IBuildAction
  {
    private readonly object _token;

    public RedirectParameterInfoBuildAction(object token = null)
    {
      _token = token;
    }

    public void Process(UnitBuilder unitBuilder)
    {
      var parameterInfo = (ParameterInfo) unitBuilder.GetUnitUnderConstruction().Id;
      var unitInfo = new UnitInfo(parameterInfo.ParameterType, _token);
      Log.Verbose("{0}: {1}", GetType().Name, unitInfo);
      unitBuilder.BuildResult = unitBuilder.Build(unitInfo);
    }

    public void PostProcess(UnitBuilder unitBuilder)
    {
    }
  }
}