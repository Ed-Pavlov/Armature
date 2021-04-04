using System;
using System.Diagnostics;
using Armature.Core.Logging;


namespace Armature.Core.BuildActions
{
  /// <summary>
  ///   Build action redirects building of unit of one type to the unit of another type. E.g. redirecting interface to the implementation
  /// </summary>
  public class RedirectTypeBuildAction : IBuildAction
  {
    private readonly Type _redirectTo;
    private readonly object? _token;

    [DebuggerStepThrough]
    public RedirectTypeBuildAction(Type redirectTo, object? token)
    {
      if (redirectTo is null) throw new ArgumentNullException(nameof(redirectTo));
      if (redirectTo.IsGenericTypeDefinition)
        throw new ArgumentException("Type should not be open generic, use RedirectOpenGenericTypeBuildAction for open generics", nameof(redirectTo));

      _redirectTo = redirectTo;
      _token = token;
    }

    public void Process(IBuildSession buildSession)
    {
      if (!buildSession.BuildResult.HasValue)
      {
        var unitUnderConstruction = buildSession.GetUnitUnderConstruction();
        var effectiveToken = Equals(_token, Token.Propagate) ? unitUnderConstruction.Token : _token;

        var unitInfo = new UnitInfo(_redirectTo, effectiveToken);
        buildSession.BuildResult = buildSession.BuildUnit(unitInfo);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _redirectTo.ToLogString());
  }
}