using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using BeatyBit.Bits;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Gets the constructor of the type with the largest number of parameters.
/// </summary>
public sealed record GetConstructorWithMaxParametersCount : IBuildAction, ILogString
{
  private readonly BindingFlags _bindingFlags;

  [PublicAPI]
  public GetConstructorWithMaxParametersCount() : this(BindingFlags.Instance | BindingFlags.Public) { }

  [PublicAPI]
  public GetConstructorWithMaxParametersCount(BindingFlags bindingFlags) => _bindingFlags = bindingFlags;

  public void Process(IBuildSession buildSession)
  {
    var unitType     = buildSession.Stack.TargetUnit.GetUnitType();
    var constructors = unitType.GetConstructors(_bindingFlags);

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
    // collect constructors with equal number of parameters to pass to the exception if something goes wrong
    var matchedConstructors = new LeanList4<ConstructorInfo>();
    var maxParametersCount  = 0;

    foreach(var constructor in constructors)
    {
      var parametersCount = constructor.GetParameters().Length;
      if(parametersCount < maxParametersCount) continue;

      if(parametersCount > maxParametersCount) // new candidate, remove previous
      {
        matchedConstructors.Clear();
        maxParametersCount = parametersCount;
      }

      matchedConstructors.Add(constructor); // parametersCount >= maxParametersCount - add to list
    }

    if(matchedConstructors.Count > 1)
    {
      var exception = new ArmatureException($"More than one constructor with max parameters count for type '{unitType.ToLogString()}' found");

      for(var i = 0; i < matchedConstructors.Count; i++)
        exception.AddData($"Constructor #{i}", matchedConstructors[i]);

      throw exception;
    }

    return matchedConstructors[0];
  }

  [DebuggerStepThrough]
  [WithoutTest]
  public override string ToString() => nameof(GetConstructorWithMaxParametersCount);

  [DebuggerStepThrough]
  [WithoutTest]
  public string ToHoconString() => Hocon.Object<GetConstructorWithMaxParametersCount>(("bindingFlags", _bindingFlags));
}
