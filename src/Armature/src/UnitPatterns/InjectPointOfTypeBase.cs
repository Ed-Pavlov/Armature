using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Base class for patterns which check if a unit is an argument for an "injection point" requires argument of the specified type
/// </summary>
public abstract record InjectPointOfTypeBase : IUnitPattern, ILogString, IInternal<IUnitPattern>
{
  private readonly IUnitPattern _typePattern;

  [DebuggerStepThrough]
  protected InjectPointOfTypeBase(IUnitPattern typePattern) => _typePattern = typePattern ?? throw new ArgumentNullException(nameof(typePattern));

  public bool Matches(UnitId unitId)
  {
    if(unitId.Tag != ServiceTag.Argument) return false;

    var type = GetInjectPointType(unitId);
    return type is not null && _typePattern.Matches(Unit.By(type));
  }

  protected abstract Type? GetInjectPointType(UnitId unitId);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} {{ TypePattern: {_typePattern.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();

  IUnitPattern IInternal<IUnitPattern>.Member1 => _typePattern;
}