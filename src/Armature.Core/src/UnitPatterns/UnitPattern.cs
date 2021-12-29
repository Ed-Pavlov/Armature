﻿using System;
using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if a unit matches with the specified kind and a tag.
/// </summary>
public sealed record UnitPattern : IUnitPattern, ILogString
{
  private readonly object? _unitKind;
  private readonly object? _tag;

  [DebuggerStepThrough]
  public UnitPattern(object? unitKind, object? tag = null)
  {
    if(unitKind is null && tag is null) throw new ArgumentNullException(nameof(unitKind), $"Either {nameof(unitKind)} or {nameof(tag)} should be provided");

    _unitKind = unitKind;
    _tag      = tag;
  }

  /// <inheritdoc />
  public bool Matches(UnitId unitId) => Equals(_unitKind, unitId.Kind) && _tag.Matches(unitId.Tag);

  [DebuggerStepThrough]
  public string ToHoconString() => $"{{ {nameof(UnitPattern)} {{ kind: {_unitKind.ToHoconString()}, tag: {_tag.ToHoconString()} }} }}";
  [DebuggerStepThrough]
  public override string ToString() => ToHoconString();
}
