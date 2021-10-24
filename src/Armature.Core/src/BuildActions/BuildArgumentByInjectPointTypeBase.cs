using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class for build actions build arguments to inject.
  /// </summary>
  public abstract record BuildArgumentByInjectPointTypeBase : IBuildAction
  {
    private readonly object? _key;

    protected BuildArgumentByInjectPointTypeBase() {}
    protected BuildArgumentByInjectPointTypeBase(object? key) => _key = key;

    public void Process(IBuildSession buildSession)
    {
      Log.WriteLine(LogLevel.Trace, "");
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();

      var effectiveKey = _key == SpecialKey.Propagate ? unitUnderConstruction.Key : _key;

      var valueType = GetInjectPointType(unitUnderConstruction);
      buildSession.BuildResult = buildSession.BuildUnit(new UnitId(valueType, effectiveKey));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract Type GetInjectPointType(UnitId unitId);

    public override string ToString() => $"{GetType().GetShortName()}{{ Key = {_key.ToLogString()} }}";
  }
}
