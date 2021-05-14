using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class for build actions build arguments to inject.
  /// </summary>
  public abstract record BuildArgumentByInjectPointNameBase : IBuildAction
  {
    private readonly object? _key;

    [DebuggerStepThrough]
    protected BuildArgumentByInjectPointNameBase() : this(SpecialKey.Argument){} // TODO: why there is default key?
    protected BuildArgumentByInjectPointNameBase(object? key) => _key = key;

    public void Process(IBuildSession buildSession)
    {
      var unitUnderConstruction = buildSession.GetUnitUnderConstruction();

      var effectiveKey = _key == SpecialKey.Propagate ? unitUnderConstruction.Key : _key;

      var injectPointName = GetInjectPointName(unitUnderConstruction);
      buildSession.BuildResult = buildSession.BuildUnit(new UnitId(injectPointName, effectiveKey));
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    protected abstract string GetInjectPointName(UnitId unitId);

    public override string ToString() => $"{GetType().GetShortName()}{{ Key = {_key.ToLogString()} }}";
  }
}
