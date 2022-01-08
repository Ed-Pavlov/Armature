using System;
using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Redirects building of a unit of one type to the unit of another type. E.g. redirecting interface to the implementation.
/// </summary>
public record RedirectType : IBuildAction, ILogString
{
  private readonly Type    _redirectTo;
  private readonly object? _tag;

  [DebuggerStepThrough]
  public RedirectType(Type redirectTo, object? tag)
  {
    if(redirectTo is null) throw new ArgumentNullException(nameof(redirectTo));

    if(redirectTo.IsGenericTypeDefinition)
      throw new ArgumentException($"Type should not be open generic, use {nameof(RedirectOpenGenericType)} for open generics", nameof(redirectTo));

    _redirectTo = redirectTo;
    _tag        = tag;
  }

  public void Process(IBuildSession buildSession)
  {
    Log.WriteLine(LogLevel.Trace, () => $"Tag: {_tag.ToHoconString()}");

    var targetUnit   = buildSession.BuildChain.TargetUnit;
    var effectiveTag = Equals(_tag, SpecialTag.Propagate) ? targetUnit.Tag : _tag;

    var unitInfo = new UnitId(_redirectTo, effectiveTag);
    buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString()
    => $"{{ {nameof(RedirectType)} {{ RedirectToType: {_redirectTo.ToLogString().QuoteIfNeeded()}, Tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}