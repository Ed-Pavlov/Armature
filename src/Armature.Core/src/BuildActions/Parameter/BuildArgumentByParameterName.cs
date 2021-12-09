using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;

namespace Armature.Core;

/// <summary>
/// Builds an argument for the constructor/method parameter using <see cref="ParameterInfo.Name"/> and specified key as <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByParameterName : BuildArgumentByInjectPointNameBase
{
  [WithoutTest]
  [DebuggerStepThrough]
  public BuildArgumentByParameterName() { }

  [WithoutTest]
  [DebuggerStepThrough]
  public BuildArgumentByParameterName(object? key) : base(key) { }

  protected override string GetInjectPointName(UnitId unitId) => ((ParameterInfo) unitId.Kind!).Name;
}