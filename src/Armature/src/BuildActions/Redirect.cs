using System.Diagnostics;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Redirects building of a unit with one <see cref="UnitId"/> to the unit with another <see cref="UnitId"/>.
/// E.g., redirecting interface to the implementation.
/// </summary>
public record Redirect : IBuildAction, ILogString
{
  private readonly UnitId _unitId;

  [DebuggerStepThrough]
  public Redirect(UnitId unitId) => _unitId = unitId;

  public void Process(IBuildSession buildSession)
  {
    var targetUnit   = buildSession.Stack.TargetUnit;
    var unitId = Equals(_unitId.Tag, ServiceTag.Propagate) ? Unit.By(_unitId.Kind, targetUnit.Tag) : _unitId;

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