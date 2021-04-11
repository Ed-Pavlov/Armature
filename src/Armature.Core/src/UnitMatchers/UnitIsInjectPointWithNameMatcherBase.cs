using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" by name
  /// </summary>
  public abstract record UnitIsInjectPointWithNameMatcherBase : IUnitIdMatcher
  {
    private readonly string _name;

    [DebuggerStepThrough]
    protected UnitIsInjectPointWithNameMatcherBase(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && GetInjectPointName(unitId) == _name;

    protected abstract string? GetInjectPointName(UnitId unitId);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _name);
  }
}
