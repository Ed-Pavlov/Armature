﻿using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Gets a constructor of type marked with <see cref="InjectAttribute" /> with the optional <see cref="InjectAttribute.InjectionPointId" />.
/// </summary>
public record GetConstructorByInjectPointId : IBuildAction, ILogString
{
  private readonly object? _injectPointId;

  public GetConstructorByInjectPointId() { }
  public GetConstructorByInjectPointId(object? injectPointId) => _injectPointId = injectPointId;

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.BuildChain.TargetUnit.GetUnitType();

    var constructors = unitType
                      .GetConstructors()
                      .Where(
                         ctor =>
                         {
                           var attribute = ctor.GetCustomAttribute<InjectAttribute>();
                           return attribute is not null && Equals(_injectPointId, attribute.InjectionPointId);
                         })
                      .ToArray();

    if(constructors.Length > 1)
    {
      var exception = new ArmatureException(
        $"More than one constructors of the type {unitType.ToLogString()} are marked with attribute "
      + $"{nameof(InjectAttribute)} with {nameof(InjectAttribute.InjectionPointId)}={_injectPointId.ToHoconString()}");

      for(var i = 0; i < constructors.Length; i++)
        exception.AddData($"Constructor #{i}", constructors[i]);

      throw exception;
    }

    var ctor = constructors.Length > 0 ? constructors[0] : null;
    ctor.WriteToLog(LogLevel.Trace);

    if(ctor is not null)
      buildSession.BuildResult = new BuildResult(constructors[0]);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(GetConstructorByInjectPointId)} {{ InjectPointId: {_injectPointId.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}