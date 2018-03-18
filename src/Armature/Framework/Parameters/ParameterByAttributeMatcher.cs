using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Parameters
{
  public class ParameterByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T> _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public ParameterByAttributeMatcher([CanBeNull] Predicate<T> predicate) => _predicate = predicate;

    public bool Matches(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is ParameterInfo parameterInfo) || unitInfo.Token != SpecialToken.InjectValue)
        return false;

      var attribute = parameterInfo
        .GetCustomAttributes(typeof(T), true)
        .OfType<T>()
        .SingleOrDefault();
      return attribute != null && (_predicate == null || _predicate(attribute));
    }

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public virtual bool Equals(IUnitMatcher matcher) => matcher is ParameterByAttributeMatcher<T> other && Equals(_predicate, other._predicate);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as ParameterByAttributeMatcher<T>);

    [DebuggerStepThrough]
    public override int GetHashCode() => _predicate != null ? _predicate.GetHashCode() : 0;
    #endregion
  }
}