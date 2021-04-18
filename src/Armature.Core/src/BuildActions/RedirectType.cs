using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Redirects building of a unit of one type to the unit of another type. E.g. redirecting interface to the implementation.
  /// </summary>
  public class RedirectType : IBuildAction
  {
    private readonly Type    _redirectTo;
    private readonly object? _key;

    [DebuggerStepThrough]
    public RedirectType(Type redirectTo, object? key)
    {
      if(redirectTo is null) throw new ArgumentNullException(nameof(redirectTo));

      if(redirectTo.IsGenericTypeDefinition)
        throw new ArgumentException("Type should not be open generic, use RedirectOpenGenericTypeBuildAction for open generics", nameof(redirectTo));

      _redirectTo = redirectTo;
      _key        = key;
    }

    public void Process(IBuildSession buildSession)
    {
      if(!buildSession.BuildResult.HasValue)
      {
        var unitUnderConstruction = buildSession.GetUnitUnderConstruction();
        var effectiveKey          = Equals(_key, SpecialKey.Propagate) ? unitUnderConstruction.Key : _key;

        var unitInfo = new UnitId(_redirectTo, effectiveKey);
        buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _redirectTo.ToLogString());
  }
}
