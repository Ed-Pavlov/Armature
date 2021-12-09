using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for build actions build arguments to inject.
/// </summary>
public abstract record BuildArgumentByInjectPointNameBase : IBuildAction, ILogString
{
  private readonly object? _key;

  [WithoutTest]
  [DebuggerStepThrough]
  protected BuildArgumentByInjectPointNameBase() : this(SpecialKey.Argument) {}

  [WithoutTest]
  [DebuggerStepThrough]
  protected BuildArgumentByInjectPointNameBase(object? key) => _key = key;

  public void Process(IBuildSession buildSession)
  {
    var unitUnderConstruction = buildSession.GetUnitUnderConstruction();

    var effectiveKey = _key == SpecialKey.Propagate ? unitUnderConstruction.Key : _key;

    var injectPointName = GetInjectPointName(unitUnderConstruction);
    buildSession.BuildResult = buildSession.BuildUnit(new UnitId(injectPointName, effectiveKey));
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  protected abstract string GetInjectPointName(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(BuildArgumentByInjectPointNameBase)}{{ Key: {_key.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}