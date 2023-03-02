using System;
using System.Diagnostics;
using Armature.Core;

namespace Armature.UnitPatterns.UnitType;

/// <summary>
/// Base class for unit patterns matching unit with a <see cref="System.Type"/>.
/// </summary>
public abstract record TypePatternBase : ILogString
{
  protected readonly object? Tag;
  protected readonly Type    Type;

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
}