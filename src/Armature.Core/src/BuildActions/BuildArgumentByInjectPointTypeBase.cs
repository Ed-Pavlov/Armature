using System;
using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
///   Base class for build actions build arguments to inject.
/// </summary>
public abstract record BuildArgumentByInjectPointTypeBase : IBuildAction, ILogString
{
  private readonly object? _key;

  [WithoutTest]
  protected BuildArgumentByInjectPointTypeBase() {}
  protected BuildArgumentByInjectPointTypeBase(object? key) => _key = key;

  public void Process(IBuildSession buildSession)
  {
    Log.WriteLine(LogLevel.Trace, "");
    var unitUnderConstruction = buildSession.GetUnitUnderConstruction();

    var effectiveKey = _key == SpecialKey.Propagate ? unitUnderConstruction.Key : _key;

    var valueType = GetInjectPointType(unitUnderConstruction);
    buildSession.BuildResult = buildSession.BuildUnit(new UnitId(valueType, effectiveKey));
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  protected abstract Type GetInjectPointType(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(BuildArgumentByInjectPointTypeBase)} {{ Key: {_key.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}