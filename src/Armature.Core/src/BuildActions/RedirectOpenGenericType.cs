using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Redirects building of a unit of one open generic type to the unit of another open generic type.
  ///   E.g. redirecting interface to the implementation
  /// </summary>
  public class RedirectOpenGenericType : IBuildAction
  {
    private readonly Type    _redirectTo;
    private readonly object? _key;

    [DebuggerStepThrough]
    public RedirectOpenGenericType(Type redirectTo, object? key)
    {
      if(redirectTo is null) throw new ArgumentNullException(nameof(redirectTo));
      if(!redirectTo.IsGenericTypeDefinition) throw new ArgumentException("Must be open generic type", nameof(redirectTo));

      _redirectTo = redirectTo;
      _key        = key;
    }

    public void Process(IBuildSession buildSession)
    {
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();
      var effectiveKey          = Equals(_key, SpecialKey.Propagate) ? unitUnderConstruction.Key : _key;

      var genericType = _redirectTo.MakeGenericType(buildSession.GetUnitUnderConstruction().GetUnitType().GetGenericArguments());
      buildSession.BuildResult = buildSession.BuildUnit(new UnitId(genericType, effectiveKey));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}[{1}, {2}]", GetType().GetShortName(), _redirectTo.ToLogString(), _key.ToLogString());
  }
}
