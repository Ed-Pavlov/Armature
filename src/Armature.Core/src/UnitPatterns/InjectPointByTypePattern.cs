using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for patterns check if a unit is an argument for an "inject point" requires argument of the specified type.
  /// </summary>
  public abstract record InjectPointByTypePattern : IUnitPattern, ILogString
  {
    private readonly Type _type;
    private readonly bool _exactMatch;

    [DebuggerStepThrough]
    protected InjectPointByTypePattern(Type type, bool exactMatch)
    {
      _type       = type ?? throw new ArgumentNullException(nameof(type));
      _exactMatch = exactMatch;
    }

    public bool Matches(UnitId unitId)
      => unitId.Key == SpecialKey.Argument
      && _exactMatch
           ? GetInjectPointType(unitId)                          == _type
           : GetInjectPointType(unitId)?.IsAssignableFrom(_type) == true;

    protected abstract Type? GetInjectPointType(UnitId unitId);

    [DebuggerStepThrough]
    public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ Type: {_type.ToLogString()}, ExactMatch: {_exactMatch} }} }}";
    [DebuggerStepThrough]
    public sealed override string ToString() => ToHoconString();
  }
}