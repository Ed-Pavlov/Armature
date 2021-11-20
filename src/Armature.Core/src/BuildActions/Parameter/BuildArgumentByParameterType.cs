using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core.Annotations;

namespace Armature.Core;

/// <summary>
///   Builds an argument for the constructor/method parameter using <see cref="ParameterInfo.ParameterType"/> and specified key as <see cref="UnitId"/>.
/// </summary>
public record BuildArgumentByParameterType : BuildArgumentByInjectPointTypeBase
{
  [WithoutTest]
  [DebuggerStepThrough]
  public BuildArgumentByParameterType() { }

  [WithoutTest]
  [DebuggerStepThrough]
  public BuildArgumentByParameterType(object? key) : base(key) { }

  protected override Type GetInjectPointType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
}