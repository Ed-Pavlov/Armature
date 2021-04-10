using System;
using System.Diagnostics;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter by exact type matching
  /// </summary>
  public record ParameterByStrictTypeMatcher : InjectPointByStrictTypeMatcher
  {
    [DebuggerStepThrough]
    public ParameterByStrictTypeMatcher(Type parameterType) : base(parameterType) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
