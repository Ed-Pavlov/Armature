using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for build actions build arguments to inject.
/// </summary>
public abstract record BuildArgumentByInjectPointNameBase : IBuildAction, ILogString
{
  private readonly object? _tag;

  [WithoutTest]
  [DebuggerStepThrough]
  protected BuildArgumentByInjectPointNameBase() : this(SpecialTag.Argument) { }

  [WithoutTest]
  [DebuggerStepThrough]
  protected BuildArgumentByInjectPointNameBase(object? tag) => _tag = tag;

  public void Process(IBuildSession buildSession)
  {
    Log.WriteLine(LogLevel.Trace, () => $"Tag: {_tag.ToHoconString()}");

    var targetUnit   = buildSession.BuildChain.TargetUnit;
    var effectiveTag = _tag == SpecialTag.Propagate ? targetUnit.Tag : _tag;

    var injectPointName = GetInjectPointName(targetUnit);
    buildSession.BuildResult = buildSession.BuildUnit(new UnitId(injectPointName, effectiveTag));
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  protected abstract string GetInjectPointName(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(BuildArgumentByInjectPointNameBase)}{{ Tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}