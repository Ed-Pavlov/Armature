using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T> _predicate;

    [DebuggerStepThrough]
    public ParameterByAttributeMatcher([CanBeNull] Predicate<T> predicate) => _predicate = predicate;

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

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => other is ParameterByAttributeMatcher<T> matcher && Equals(_predicate, matcher._predicate);
  }
}