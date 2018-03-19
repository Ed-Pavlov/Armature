using System;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Parameters
{
  public class ParameterByValueMatcher : InjectPointByValueMatcher
  {
    public ParameterByValueMatcher([NotNull] object parameterValue) : base(parameterValue)
    {
    }

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.ParameterType;
  }
}