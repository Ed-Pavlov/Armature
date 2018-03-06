using System;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByNameMatcher : IUnitMatcher
  {
    private readonly string _parameterName;

    public ParameterByNameMatcher([NotNull] string parameterName)
    {
      if (parameterName == null) throw new ArgumentNullException("parameterName");
      _parameterName = parameterName;
    }


    public bool Matches(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;
      return parameterInfo != null && unitInfo.Token == SpecialToken.ParameterValue && parameterInfo.Name == _parameterName;
    }

    public bool Equals(IUnitMatcher other)
    {
      var matcher = other as ParameterByNameMatcher;
      return matcher != null && _parameterName == matcher._parameterName;
    }
  }
}