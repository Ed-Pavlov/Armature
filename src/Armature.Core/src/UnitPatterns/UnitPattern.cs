using System;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit matches with the specified kind and a key.
  /// </summary>
  public sealed record UnitPattern : IUnitPattern, ILogString
  {
    private readonly object? _unitKind;
    private readonly object? _key;

    [DebuggerStepThrough]
    public UnitPattern(object? unitKind, object? key = null)
    {
      if(unitKind is null && key is null) throw new ArgumentNullException(nameof(unitKind), "Either unit kind or key should be provided");

      _unitKind = unitKind;
      _key    = key;
    }

    /// <inheritdoc />
    public bool Matches(UnitId unitId) => Equals(_unitKind, unitId.Kind) && _key.Matches(unitId.Key);

    [DebuggerStepThrough]
    public string ToHoconString() => $"{{ {nameof(UnitPattern)} {{ kind: {_unitKind.ToHoconString()}, key: {_key.ToHoconString()} }} }}";
    [DebuggerStepThrough]
    public override string ToString() => ToHoconString();
  }
}