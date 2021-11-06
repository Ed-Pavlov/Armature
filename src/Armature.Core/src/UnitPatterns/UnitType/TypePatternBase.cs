using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for matchers matching unit with a <see cref="System.Type"/> pattern
  /// </summary>
  public abstract record TypePatternBase(Type Type, object? Key) : ILogString
  {
    protected readonly Type Type = Type ?? throw new ArgumentNullException(nameof(Type));

    [DebuggerStepThrough]
    public string ToHoconString() => $"{{ {GetType().GetShortName().QuoteIfNeeded()} "
                                 + $"{{ Type: {Type.ToLogString().QuoteIfNeeded()}, Key: {Key.ToHoconString()} }} }}";
    [DebuggerStepThrough]
    public sealed override string ToString() => ToHoconString();
  }
}