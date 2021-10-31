using System;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for matchers matching unit with a <see cref="System.Type"/> pattern
  /// </summary>
  public abstract record TypePatternBase(Type Type, object? Key) : ILogable1
  {
    protected readonly Type    Type = Type ?? throw new ArgumentNullException(nameof(Type));
    protected readonly object? Key  = Key;

    public string ToLogString() => $"{{ {GetType().GetShortName()} "
                                 + $"{{ Type: {Type.ToLogString().QuoteIfNeeded()}, Key: {Key.ToLogString().QuoteIfNeeded() } }} }}";
  }
}