using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Redirects building of a unit of one open generic type to the unit of another open generic type.
/// E.g. redirecting interface to the implementation
/// </summary>
public record RedirectOpenGenericType : IBuildAction, ILogString
{
  private readonly Type    _redirectTo;
  private readonly object? _tag;
  private readonly bool    _throwOnMismatch;

  [DebuggerStepThrough]
  public RedirectOpenGenericType(Type redirectTo, object? tag, bool throwOnMismatch = true)
  {
    if(redirectTo is null) throw new ArgumentNullException(nameof(redirectTo));
    if(!redirectTo.IsGenericTypeDefinition) throw new ArgumentException("Must be open generic type", nameof(redirectTo));

    _redirectTo      = redirectTo;
    _tag             = tag;
    _throwOnMismatch = throwOnMismatch;
  }

  public void Process(IBuildSession buildSession)
  {
    Log.WriteLine(LogLevel.Trace, () => $"Tag: {_tag.ToHoconString()}");

    var targetUnit = buildSession.Stack.TargetUnit;
    var unitType   = targetUnit.GetUnitType();

    if(!unitType.IsGenericType)
      if(_throwOnMismatch)
        throw new ArmatureException($"Building unit {targetUnit} is not a generic type and can't be redirected.");
      else
        return;

    if(unitType.IsGenericTypeDefinition)
      if(_throwOnMismatch)
        throw new ArmatureException($"Building unit {targetUnit} is an open generic type and can't be redirected.");
      else
        return;

    var genericArguments = unitType.GetGenericArguments();

    if(_redirectTo.GetTypeInfo().GenericTypeParameters.Length != genericArguments.Length)
      if(_throwOnMismatch)
        throw new ArmatureException(
          $"Generic arguments count of building unit {targetUnit} and the type to redirect {_redirectTo} should be equal.");
      else
        return;

    var effectiveTag = Equals(_tag, SpecialTag.Propagate) ? targetUnit.Tag : _tag;
    var genericType  = _redirectTo.MakeGenericType(genericArguments);

    buildSession.BuildResult = buildSession.BuildUnit(Unit.Of(genericType, effectiveTag));
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString()
    => $"{{ {nameof(RedirectOpenGenericType)} {{ RedirectToType: {_redirectTo.ToLogString().QuoteIfNeeded()}, Tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}