using System;
using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for build actions build arguments to inject.
/// </summary>
public abstract record BuildArgumentByInjectPointTypeBase : IBuildAction, ILogString
{
  private readonly object? _tag;

  [WithoutTest]
  protected BuildArgumentByInjectPointTypeBase() { }
  protected BuildArgumentByInjectPointTypeBase(object? tag) => _tag = tag;

  public void Process(IBuildSession buildSession)
  {
    Log.WriteLine(LogLevel.Trace, "");
    var unitUnderConstruction = buildSession.BuildChain.TargetUnit;

    var effectiveTag = _tag == SpecialTag.Propagate ? unitUnderConstruction.Tag : _tag;

    var valueType = GetInjectPointType(unitUnderConstruction);
    buildSession.BuildResult = buildSession.BuildUnit(new UnitId(valueType, effectiveTag));
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  protected abstract Type GetInjectPointType(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(BuildArgumentByInjectPointTypeBase)} {{ Tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}