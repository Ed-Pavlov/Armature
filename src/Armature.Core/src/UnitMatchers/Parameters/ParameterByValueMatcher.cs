using System;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter suited for provided value type
  /// </summary>
  public record ParameterByValueMatcher : InjectPointByValueMatcher
  {
    public ParameterByValueMatcher(object parameterValue) : base(parameterValue) { }

    protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as ParameterInfo)?.ParameterType;
  }
}
