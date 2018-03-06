using System;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByStrictTypeMatcher : IUnitMatcher
  {
    private readonly Type _parameterType;

    public ParameterByStrictTypeMatcher([NotNull] Type parameterType)
    {
      if (parameterType == null) throw new ArgumentNullException("parameterType");
      _parameterType = parameterType;
    }


    public bool Matches(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;
      return parameterInfo != null && unitInfo.Token == SpecialToken.ParameterValue && parameterInfo.ParameterType == _parameterType;
    }

    public bool Equals(IUnitMatcher other)
    {
      var matcher = other as ParameterByStrictTypeMatcher;
      return matcher != null && _parameterType == matcher._parameterType;
    }
  }
}