﻿using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  /// Gets a default parameter value of the method parameter.
  /// </summary>
  public record GetParameterDefaultValue : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
      if(buildSession.GetUnitUnderConstruction().Kind is ParameterInfo {HasDefaultValue: true} parameterInfo)
        buildSession.BuildResult = new BuildResult(parameterInfo.DefaultValue);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => nameof(GetParameterDefaultValue);
  }
}