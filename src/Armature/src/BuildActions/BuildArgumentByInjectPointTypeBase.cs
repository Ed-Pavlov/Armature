using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Annotations;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Base class for build actions building arguments to inject into inject points marked with <see cref="InjectAttribute"/>.
/// </summary>
public abstract record BuildArgumentByInjectPointTypeBase : IBuildAction, ILogString
{
  private readonly object? _tag;

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
    buildSession.BuildResult = buildSession.BuildUnit(Unit.Of(valueType, effectiveTag));
  }
  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  protected abstract Type GetInjectPointType(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().ToHoconString()} {{ Tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}