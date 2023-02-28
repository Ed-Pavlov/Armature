using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for build actions building arguments to inject.
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
    if(Log.IsEnabled(LogLevel.Trace))
      Log.WriteLine(LogLevel.Trace, $"Tag: {_tag.ToHoconString()}");

    var targetUnit   = buildSession.Stack.TargetUnit;
    var effectiveTag = _tag == SpecialTag.Propagate ? targetUnit.Tag : _tag;

    var injectPointName = GetInjectPointName(targetUnit);
    buildSession.BuildResult = buildSession.BuildUnit(Unit.Of(injectPointName, effectiveTag));
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