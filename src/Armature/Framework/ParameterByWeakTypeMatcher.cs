using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public class ParameterByWeakTypeMatcher : IUnitMatcher
  {
    private readonly object _parameterValue;

    public ParameterByWeakTypeMatcher(object parameterValue)
    {
      _parameterValue = parameterValue;
    }

    public bool Matches(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;
      return parameterInfo != null && unitInfo.Token == SpecialToken.ParameterValue && parameterInfo.ParameterType.IsInstanceOfType(_parameterValue);
    }

    public bool Equals(IUnitMatcher other)
    {
      var matcher = other as ParameterByWeakTypeMatcher;
      return matcher != null && Equals(_parameterValue, matcher._parameterValue);
    }
  }
}