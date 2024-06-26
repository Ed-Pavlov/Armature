﻿using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Gets the constructor of the type which is marked with <see cref="InjectAttribute" /> the optional <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />.
/// </summary>
public record GetConstructorByInjectPoint : IBuildAction, ILogString
{
  private readonly BindingFlags _bindingFlags;
  private readonly object? _injectPointTag;

  public GetConstructorByInjectPoint() : this(BindingFlags.Instance | BindingFlags.Public) { }
  public GetConstructorByInjectPoint(BindingFlags bindingFlags) : this(null, bindingFlags) { }
  public GetConstructorByInjectPoint(object? injectPointTag, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
  {
    _bindingFlags = bindingFlags;
    _injectPointTag = injectPointTag;
  }

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.Stack.TargetUnit.GetUnitType();

    var constructors = unitType
                      .GetConstructors(_bindingFlags)
                      .Where(
                         ctor =>
                         {
                           var attributes = ctor.GetCustomAttributes<InjectAttribute>();
                           return attributes.Any(attribute => Equals(_injectPointTag, attribute.Tag));
                         })
                      .ToArray();

    if(constructors.Length > 1)
    {
      var exception = new ArmatureException(
        $"More than one constructors of the type {unitType.ToLogString()} are marked with attribute "
      + $"{nameof(InjectAttribute)} with {nameof(InjectAttribute.Tag)}={_injectPointTag.ToHoconString()}");

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
  public string ToHoconString() => $"{{ {nameof(GetConstructorByInjectPoint)} {{ InjectPointId: {_injectPointTag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
