using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature;

/// <summary>
/// Base class for patterns which check if a unit is an argument for an "injection point" with the specified name
/// </summary>
public abstract record InjectPointNamedBase : IUnitPattern, ILogString, IInternal<string>
{
  [PublicAPI]
  protected readonly string _name;

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

  string IInternal<string>.Member1 => _name;
}