using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Sdk;

namespace Armature.UnitPatterns;

/// <summary>
/// Base class for patterns check if a unit is an argument for an "inject point" with the specified name.
/// </summary>
public abstract record InjectPointNamedBase : IUnitPattern, ILogString
{
  private readonly string _name;

  [DebuggerStepThrough]
  protected InjectPointNamedBase(string name)
  {
    if(string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
    _name = name;
  }

  public bool Matches(UnitId unitId) => unitId.Tag == ServiceTag.Argument && GetInjectPointName(unitId) == _name;

  protected abstract string? GetInjectPointName(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ Name: {_name.QuoteIfNeeded()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}