using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core.Annotations;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Builds an argument for the constructor/method parameter using <see cref="ParameterInfo.Name"/> and specified tag as <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByParameterName : BuildArgumentByInjectPointNameBase
{
  [WithoutTest]
  [DebuggerStepThrough]
  public BuildArgumentByParameterName() { }

  [WithoutTest]
  [DebuggerStepThrough]
  public BuildArgumentByParameterName(object? tag) : base(tag) { }

  protected override string GetInjectPointName(UnitId unitId) => ((ParameterInfo) unitId.Kind!).Name;
}
