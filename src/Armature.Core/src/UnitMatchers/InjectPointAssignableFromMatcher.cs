using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Base class for matchers matching if an "inject point" can accept argument of the specified type
  /// </summary>
  public abstract record InjectPointAssignableFromMatcher : IUnitIdMatcher
  {
    private readonly Type _type;

    [DebuggerStepThrough]
    protected InjectPointAssignableFromMatcher(Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.InjectValue && GetInjectPointType(unitId)?.IsAssignableFrom(_type) == true;

    protected abstract Type? GetInjectPointType(UnitId unitId);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type.ToLogString());
  }
}
