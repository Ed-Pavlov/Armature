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
    private readonly object? _key;

    [DebuggerStepThrough]
    protected CreateValueToInjectBuildAction(object? key) => _key = key;

    public void Process(IBuildSession buildSession)
    {
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();

      var effectiveKey = _key == SpecialKey.Propagate ? unitUnderConstruction.Key : _key;

      var valueType = GetValueType(unitUnderConstruction);
      buildSession.BuildResult = buildSession.BuildUnit(new UnitId(valueType, effectiveKey));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract Type GetValueType(UnitId unitId);

    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _key.ToLogString());
  }
}
