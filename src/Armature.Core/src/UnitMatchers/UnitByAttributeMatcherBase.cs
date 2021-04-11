using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" marked with attribute which satisfies user provided conditions
  /// </summary>
  public abstract record UnitByAttributeMatcherBase<T> : IUnitIdMatcher where T : Attribute
  {
    private readonly Predicate<T>? _predicate;

    [DebuggerStepThrough]
    protected UnitByAttributeMatcherBase(Predicate<T>? predicate) => _predicate = predicate;

    public bool Matches(UnitId unitId)
    {
      if(unitId.Key != SpecialKey.InjectValue) return false;

      var attribute = GetAttribute(unitId);
      return attribute is not null && (_predicate is null || _predicate(attribute));
    }

    protected abstract T? GetAttribute(UnitId unitId);
    
    public override string ToString() => GetType().GetShortName();
  }
}
