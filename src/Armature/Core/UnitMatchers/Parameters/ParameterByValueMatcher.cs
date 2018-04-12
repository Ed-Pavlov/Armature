using System;
using System.Reflection;
using Resharper.Annotations;

namespace Armature.Core.UnitMatchers.Parameters
{
  public class ParameterByValueMatcher : InjectPointByValueMatcher
  {
    public ParameterByValueMatcher([NotNull] object parameterValue) : base(parameterValue)
    {
    }

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.ParameterType;
  }
}