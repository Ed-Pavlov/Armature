using System.Diagnostics;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Redirects building of a unit of one type to the unit of another type. E.g. redirecting interface to the implementation.
/// </summary>
public record Redirect : IBuildAction, ILogString
{
  private readonly UnitId _unitId;

  [DebuggerStepThrough]
  public Redirect(UnitId unitId) => _unitId = unitId;

  public void Process(IBuildSession buildSession)
  {
    var targetUnit   = buildSession.Stack.TargetUnit;
    var unitId = Equals(_unitId.Tag, SpecialTag.Propagate) ? new UnitId(_unitId.Kind, targetUnit.Tag) : _unitId;

    buildSession.BuildResult = buildSession.BuildUnit(unitId);
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public void PostProcess(IBuildSession buildSession) { }

  [DebuggerStepThrough]
  public string ToHoconString()
    => $"{{ {nameof(Redirect)} {{ UnitId: {_unitId.ToHoconString()} }} }}";

  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}