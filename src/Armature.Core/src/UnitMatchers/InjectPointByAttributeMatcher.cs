using System;
using System.Diagnostics;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" marked with attribute which satisfies user provided conditions
  /// </summary>
  public abstract record InjectPointByAttributeMatcher<T> : UnitMatcherBase, IUnitMatcher
  {
    private readonly Predicate<T>? _predicate;

    [DebuggerStepThrough]
    protected InjectPointByAttributeMatcher(Predicate<T>? predicate) => _predicate = predicate;

    public bool Matches(UnitId unitId)
    {
      if(unitId.Key != SpecialToken.InjectValue) return false;

      var attribute = GetInjectPointAttribute(unitId);
      return attribute is not null && (_predicate is null || _predicate(attribute));
    }

    protected abstract T? GetInjectPointAttribute(UnitId unitId);
  }
}
