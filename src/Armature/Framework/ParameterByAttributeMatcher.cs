using System;
using System.Linq;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T> _predicate;

    public ParameterByAttributeMatcher([CanBeNull] Predicate<T> predicate)
    {
      _predicate = predicate;
    }

    public bool Matches(UnitInfo unitInfo)
    {
      var parameterInfo = unitInfo.Id as ParameterInfo;
      if (parameterInfo == null || unitInfo.Token != SpecialToken.ParameterValue)
        return false;
      var attribute = parameterInfo
        .GetCustomAttributes(typeof(T), true)
        .OfType<T>()
        .SingleOrDefault();
      return attribute != null && (_predicate == null || _predicate(attribute));
    }

    public bool Equals(IUnitMatcher other)
    {
      var matcher = other as ParameterByAttributeMatcher<T>;
      return matcher != null && Equals(_predicate, matcher._predicate);
    }
  }
}