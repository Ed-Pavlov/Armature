using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework
{
  public class ParameterByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T> _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public ParameterByAttributeMatcher([CanBeNull] Predicate<T> predicate) => _predicate = predicate;

    public bool Matches(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is ParameterInfo parameterInfo) || unitInfo.Token != SpecialToken.ParameterValue)
        return false;

      var attribute = parameterInfo
        .GetCustomAttributes(typeof(T), true)
        .OfType<T>()
        .SingleOrDefault();
      return attribute != null && (_predicate == null || _predicate(attribute));
    }

    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher matcher) => matcher is ParameterByAttributeMatcher<T> other && Equals(_predicate, other._predicate);
    
    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();
  }
}