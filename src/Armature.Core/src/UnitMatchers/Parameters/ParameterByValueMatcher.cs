using System;
using System.Reflection;

namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter suited for provided value type
  /// </summary>
  public sealed record ParameterByValueMatcher : InjectPointByValueMatcher
  {
    public ParameterByValueMatcher(object parameterValue) : base(parameterValue) { }

    protected override Type? GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.ParameterType;
  }
}