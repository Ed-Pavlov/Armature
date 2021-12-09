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
  private readonly object? _key;

  [DebuggerStepThrough]
  public RedirectType(Type redirectTo, object? key)
  {
    if(redirectTo is null) throw new ArgumentNullException(nameof(redirectTo));

    if(redirectTo.IsGenericTypeDefinition)
      throw new ArgumentException($"Type should not be open generic, use {nameof(RedirectOpenGenericType)} for open generics", nameof(redirectTo));

    _redirectTo = redirectTo;
    _key        = key;
  }

  public void Process(IBuildSession buildSession)
  {
    var unitUnderConstruction = buildSession.BuildChain.TargetUnit;
    var effectiveKey          = Equals(_key, SpecialKey.Propagate) ? unitUnderConstruction.Key : _key;

    var unitInfo = new UnitId(_redirectTo, effectiveKey);
    buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString()
    => $"{{ {nameof(RedirectType)} {{ RedirectToType: {_redirectTo.ToLogString().QuoteIfNeeded()}, Key: {_key.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}