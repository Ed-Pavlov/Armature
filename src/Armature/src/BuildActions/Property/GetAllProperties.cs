using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

public class GetAllProperties : IBuildAction, ILogString
{
  private readonly BindingFlags              _bindingFlags;
  private readonly Func<PropertyInfo, bool>? _predicate;

  public GetAllProperties(BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public, Func<PropertyInfo, bool>? predicate = null)
  {
    _bindingFlags = bindingFlags;
    _predicate    = predicate;
  }

  public void Process(IBuildSession buildSession)
  {
    var unitType = buildSession.Stack.TargetUnit.GetUnitType();

    var properties = unitType.GetProperties(_bindingFlags);

    if(_predicate is not null)
      properties = properties.Where(_predicate).ToArray();

    buildSession.BuildResult = new BuildResult(properties);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
  [DebuggerStepThrough]
  public string ToHoconString() => Hocon.Object<GetAllProperties>(("bindingFlags", _bindingFlags), ("predicate", _predicate is null ? "not set" : "set"));
}
