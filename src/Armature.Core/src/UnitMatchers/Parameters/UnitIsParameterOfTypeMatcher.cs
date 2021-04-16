using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Matches parameter by exact type matching
  /// </summary>
  public record UnitIsParameterOfTypeMatcher : UnitIsInjectPointOfTypeMatcher
  {
    [DebuggerStepThrough]
    public UnitIsParameterOfTypeMatcher(Type parameterType) : base(parameterType) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
