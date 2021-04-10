using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" suited for provided value type
  /// </summary>
  public abstract record InjectPointByValueMatcher : IUnitMatcher
  {
    private readonly object _value;

    [DebuggerStepThrough]
    protected InjectPointByValueMatcher(object parameterValue) => _value = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));

    public bool Matches(UnitId unitId)
    {
      var type = GetInjectPointType(unitId);

      return unitId.Key == SpecialToken.InjectValue && type is not null && type.IsInstanceOfType(_value);
    }

    protected abstract Type? GetInjectPointType(UnitId unitId);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _value.ToLogString());
  }
}
