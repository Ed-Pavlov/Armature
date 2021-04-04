using System;
using System.Diagnostics;
using System.Reflection;


namespace Armature.Core.UnitMatchers.Parameters
{
  /// <summary>
  ///   Matches parameter by exact type matching
  /// </summary>
  public class ParameterByStrictTypeMatcher : InjectPointByStrictTypeMatcher
  {
    [DebuggerStepThrough]
    public ParameterByStrictTypeMatcher(Type parameterType) : base(parameterType) { }

    protected override Type? GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.ParameterType;
  }
}