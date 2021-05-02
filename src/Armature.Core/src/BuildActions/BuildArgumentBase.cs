using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class for build actions build arguments to inject.
  /// </summary>
  public abstract class BuildArgumentBase : IBuildAction
  {
    private readonly object? _key;

    [DebuggerStepThrough]
    protected BuildArgumentBase(object? key) => _key = key;

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

    public override string ToString() => $"{GetType().GetShortName()}{{ Key = {_key.ToLogString()} }}";
  }
}
