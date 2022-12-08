using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Gets the constructor of the type with the largest number of parameters.
/// </summary>
public record GetConstructorWithMaxParametersCount : IBuildAction
{
  public void Process(IBuildSession buildSession)
  {
    var unitType     = buildSession.Stack.TargetUnit.GetUnitType();
    var constructors = unitType.GetConstructors();

    if(constructors.Length > 0)
    {
      var ctor = GetConstructor(constructors, unitType);
      ctor.WriteToLog(LogLevel.Trace);
      buildSession.BuildResult = new BuildResult(ctor);
    }
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  private static ConstructorInfo GetConstructor(IReadOnlyList<ConstructorInfo> constructors, Type unitType)
  {
    var suitableConstructors = new Dictionary<int, int> {{0, constructors[0].GetParameters().Length}};
    for(var i = 1; i < constructors.Count; i++)
    {
      var parametersCount    = constructors[i].GetParameters().Length;
      var maxParametersCount = suitableConstructors.First().Value;

      if(parametersCount == maxParametersCount)
        suitableConstructors.Add(i, parametersCount);
      else if(parametersCount > maxParametersCount)
      {
        suitableConstructors.Clear();
        suitableConstructors.Add(i, parametersCount);
      }
    }

    if(suitableConstructors.Count > 1)
    {
      var exception = new ArmatureException($"More than one constructor with max parameters count for type '{unitType.ToLogString()}' found");

      var counter = 0;

      foreach(var pair in suitableConstructors)
        exception.AddData($"Constructor #{++counter}", constructors[pair.Key]);

      throw exception;
    }

    return constructors[suitableConstructors.First().Key];
  }

  [DebuggerStepThrough]
  public override string ToString() => nameof(GetConstructorWithMaxParametersCount);
}