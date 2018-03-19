using System;
using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Properties;

namespace Armature.Framework.UnitMatchers.Parameters
{
  public class ParameterByStrictTypeMatcher : InjectPointByStrictTypeMatcher
  {
    [DebuggerStepThrough]
    public ParameterByStrictTypeMatcher([NotNull] Type parameterType) : base(parameterType){}

    protected override Type GetInjectPointType(UnitInfo unitInfo) => (unitInfo.Id as ParameterInfo)?.ParameterType;
  }
}