using System;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Base class for matchers matching unit with a <see cref="System.Type"/> pattern
/// </summary>
public abstract record TypePatternBase : ILogString
{
  protected readonly   object? Key;
  protected readonly Type    Type;

  /// <summary>
  /// Base class for matchers matching unit with a <see cref="System.Type"/> pattern
  /// </summary>
  protected TypePatternBase(Type type, object? key)
  {
    Type = type ?? throw new ArgumentNullException(nameof(type));
    Key = key;
  }

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} "
                                 + $"{{ Type: {Type.ToLogString().QuoteIfNeeded()}, Key: {Key.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public sealed override string ToString() => ToHoconString();
}