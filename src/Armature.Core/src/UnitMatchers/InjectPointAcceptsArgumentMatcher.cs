using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" suited for provided value type
  /// </summary>
  public abstract record InjectPointAcceptsArgumentMatcher : IUnitIdMatcher
  {
    private readonly object _argument;

    [DebuggerStepThrough]
    protected InjectPointAcceptsArgumentMatcher(object argument) => _argument = argument ?? throw new ArgumentNullException(nameof(argument));

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && GetInjectPointType(unitId)?.IsInstanceOfType(_argument) == true;

    protected abstract Type? GetInjectPointType(UnitId unitId);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _argument.ToLogString());
  }
}
