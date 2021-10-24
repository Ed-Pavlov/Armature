﻿using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Checks if a unit matches with the specified kind and a key.
  /// </summary>
  public sealed record Pattern : IUnitPattern
  {
    private readonly object? _unitKind;
    private readonly object? _key;

    [DebuggerStepThrough]
    public Pattern(object? unitKind, object? key)
    {
      if(unitKind is null && key is null) throw new ArgumentNullException(nameof(unitKind), @"Either id or key should be provided");

      _unitKind = unitKind;
      _key    = key;
    }

    /// <inheritdoc />
    public bool Matches(UnitId unitId) => Equals(_unitKind, unitId.Kind) && _key.Matches(unitId.Key);

    [DebuggerStepThrough]
    public override string ToString() => string.Format("{0}({1}:{2})", GetType().GetShortName(), _unitKind.ToLogString(), _key.ToLogString());

    public string ToLogString() => $"{{ kind: {_unitKind.ToLogString()}, key: {_key.ToLogString()} }}";
  }
}
