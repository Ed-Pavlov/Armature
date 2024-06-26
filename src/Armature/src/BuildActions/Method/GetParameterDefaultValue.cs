﻿using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Gets a default parameter value of the constructor/method parameter.
/// </summary>
public record GetParameterDefaultValue : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    if(buildSession.Stack.TargetUnit.Kind is not ParameterInfo parameterInfo)
      Log.WriteLine(LogLevel.Trace, "ParameterInfo: null");
    else if(!parameterInfo.HasDefaultValue)
      Log.WriteLine(LogLevel.Trace, "HasDefaultValue: false");
    else
      buildSession.BuildResult = new BuildResult(parameterInfo.DefaultValue);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => nameof(GetParameterDefaultValue);
}