using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for matchers matching if a building unit is an argument for an "inject point"
  /// </summary>
  public abstract record IsInjectPointOfTypePattern : IUnitIdPattern
  {
    private readonly Type _type;
    private readonly bool _exactMatch;

    [DebuggerStepThrough]
    protected IsInjectPointOfTypePattern(Type type, bool exactMatch)
    {
      _type       = type ?? throw new ArgumentNullException(nameof(type));
      _exactMatch = exactMatch;
    }

    public bool Matches(UnitId unitId)
      => unitId.Key == SpecialKey.InjectValue
      && _exactMatch
           ? GetInjectPointType(unitId)                          == _type
           : GetInjectPointType(unitId)?.IsAssignableFrom(_type) == true;

    protected abstract Type? GetInjectPointType(UnitId unitId);

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _type.ToLogString());
  }
}
