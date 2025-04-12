using System;
using System.Diagnostics;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Base class for unit patterns matching unit with a <see cref="System.Type"/>.
/// </summary>
public abstract record TypePatternBase : ILogString, IInternal<Type, object?>
{
  protected readonly Type    _type;
  protected readonly object? _tag;

  /// <summary>
  /// Base class for unit patterns matching unit with a <see cref="System.Type"/>
  /// </summary>
  protected TypePatternBase(Type type, object? tag)
  {
    _type = type ?? throw new ArgumentNullException(nameof(type));
    _tag  = tag;
  }

  [DebuggerStepThrough]
  public string ToHoconString() => Hocon.Object(GetType(), ("type", _type), ("tag", _tag));
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();

  #region Internals

  Type IInternal<Type>.            Member1 => _type;
  object? IInternal<Type, object?>.Member2 => _tag;

  #endregion
}
