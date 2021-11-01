using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for patterns check if a unit is an argument for an "inject point" with the specified name.
  /// </summary>
  public abstract record InjectPointWithNamePattern : IUnitPattern, ILogString
  {
    private readonly string _name;

    [DebuggerStepThrough]
    protected InjectPointWithNamePattern(string name) => _name = name ?? throw new ArgumentNullException(nameof(name));

    public bool Matches(UnitId unitId) => unitId.Key == SpecialKey.Argument && GetInjectPointName(unitId) == _name;

    protected abstract string? GetInjectPointName(UnitId unitId);

    [DebuggerStepThrough]
    public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ Name: {_name.QuoteIfNeeded()} }} }}";
    [DebuggerStepThrough]
    public sealed override string ToString() => ToHoconString();
  }
}