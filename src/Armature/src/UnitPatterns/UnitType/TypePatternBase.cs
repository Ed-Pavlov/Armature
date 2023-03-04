using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature.UnitPatterns.UnitType;

/// <summary>
/// Base class for unit patterns matching unit with a <see cref="System.Type"/>.
/// </summary>
public abstract record TypePatternBase : ILogString, IInternal<Type, object?>
{
  protected readonly Type    Type;
  protected readonly object? Tag;

  /// <summary>
  /// Base class for matchers matching unit with a <see cref="System.Type"/> pattern
  /// </summary>
  protected TypePatternBase(Type type, object? tag)
  {
    Type = type ?? throw new ArgumentNullException(nameof(type));
    Tag  = tag;
  }

  [DebuggerStepThrough]
  public string ToHoconString()
    => $"{{ {GetType().GetShortName().QuoteIfNeeded()} "
     + $"{{ Type: {Type.ToLogString().QuoteIfNeeded()}, Tag: {Tag.ToHoconString()} }} }}";

  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();

  #region Internals
  Type IInternal<Type>.            Member1 => Type;
  object? IInternal<Type, object?>.Member2 => Tag;
  #endregion
}