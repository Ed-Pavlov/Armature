using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" marked with attribute which satisfies user provided conditions
  /// </summary>
  public abstract class InjectPointByAttributeMatcher<T> : IUnitMatcher
  {
    private readonly Predicate<T>? _predicate;

    [DebuggerStepThrough]
    protected InjectPointByAttributeMatcher(Predicate<T>? predicate) => _predicate = predicate;

    public bool Matches(UnitInfo unitInfo)
    {
      if (unitInfo.Token != SpecialToken.InjectValue) return false;

      var attribute = GetInjectPointAttribute(unitInfo);
      return attribute != null && (_predicate == null || _predicate(attribute));
    }

    protected abstract T? GetInjectPointAttribute(UnitInfo unitInfo);

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public virtual bool Equals(IUnitMatcher? obj) =>
      obj is InjectPointByAttributeMatcher<T> other && GetType() == obj.GetType() && Equals(_predicate, other._predicate);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => _predicate != null ? _predicate.GetHashCode() : 0;
    #endregion
  }
}