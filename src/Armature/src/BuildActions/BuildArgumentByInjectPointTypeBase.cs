using System;
using System.Diagnostics;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Base class for build actions building arguments to inject into injection points marked with <see cref="InjectAttribute"/>.
/// </summary>
public abstract record BuildArgumentByInjectPointTypeBase : IBuildAction, ILogString
{
  [PublicAPI]
  protected readonly object? _tag;

  [WithoutTest]
  protected BuildArgumentByInjectPointTypeBase() { }
  protected BuildArgumentByInjectPointTypeBase(object? tag) => _tag = tag;

  public void Process(IBuildSession buildSession)
  {
    if(Log.IsEnabled(LogLevel.Trace))
      Log.WriteLine(LogLevel.Trace, $"Tag: {_tag.ToHoconString()}");

    var targetUnit   = buildSession.Stack.TargetUnit;
    var effectiveTag = _tag == ServiceTag.Propagate ? targetUnit.Tag : _tag;

    var valueType = GetInjectPointType(targetUnit);
    buildSession.BuildResult = buildSession.BuildUnit(Unit.By(valueType, effectiveTag));
  }
  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  protected abstract Type GetInjectPointType(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => Hocon.Object(GetType(), ("tag", _tag));
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}