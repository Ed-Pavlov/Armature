using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions
{
  /// <summary>
  ///   Base class for build actions building values to inject
  /// </summary>
  public abstract class CreateValueToInjectBuildAction : IBuildAction
  {
    private readonly object? _token;

    [DebuggerStepThrough]
    protected CreateValueToInjectBuildAction(object? token) => _token = token;

    public void Process(IBuildSession buildSession)
    {
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();

      var effectiveToken = _token == Token.Propagate ? unitUnderConstruction.Token : _token;

      var valueType = GetValueType(unitUnderConstruction);
      buildSession.BuildResult = buildSession.BuildUnit(new UnitInfo(valueType, effectiveToken));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract Type GetValueType(UnitInfo unitInfo);

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _token.ToLogString());
  }
}