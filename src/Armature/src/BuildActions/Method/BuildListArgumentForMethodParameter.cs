using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Core.Annotations;

namespace Armature;

/// <summary>
/// Builds a list of arguments by using <see cref="IBuildSession.BuildAllUnits"/> method for a constructor/method parameters.
/// </summary>
public record BuildListArgumentForMethodParameter : BuildListArgumentBase
{
  [WithoutTest]
  [DebuggerStepThrough]
  public BuildListArgumentForMethodParameter() { }

  [DebuggerStepThrough]
  public BuildListArgumentForMethodParameter(object? tag) : base(tag) { }

  protected override Type GetArgumentType(UnitId unitId) => ((ParameterInfo) unitId.Kind!).ParameterType;
}
