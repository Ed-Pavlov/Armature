using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Logging;
using JetBrains.Annotations;

namespace Armature.Framework.Properties
{
  public class PropertyByAttributeMatcher<T> : IUnitMatcher
    where T : Attribute
  {
    private readonly Predicate<T> _predicate;

    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    [DebuggerStepThrough]
    public PropertyByAttributeMatcher([CanBeNull] Predicate<T> predicate) => _predicate = predicate;

    public bool Matches(UnitInfo unitInfo)
    {
      if (!(unitInfo.Id is PropertyInfo propertyInfo) || unitInfo.Token != SpecialToken.InjectValue)
        return false;

      var attribute = propertyInfo
        .GetCustomAttributes<T>()
        .SingleOrDefault();
      return attribute != null && (_predicate == null || _predicate(attribute));
    }

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public virtual bool Equals(IUnitMatcher matcher) => matcher is PropertyByAttributeMatcher<T> other && Equals(_predicate, other._predicate);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as PropertyByAttributeMatcher<T>);

    [DebuggerStepThrough]
    public override int GetHashCode() => _predicate != null ? _predicate.GetHashCode() : 0;
    #endregion
  }
}