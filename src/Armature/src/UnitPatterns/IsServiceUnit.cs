﻿using System.Diagnostics;
using Armature.Core;

namespace Armature.UnitPatterns;

/// <summary>
/// Checks if a unit is a "service" unit, constructor, argument, etc.
/// </summary>
public record IsServiceUnit : IUnitPattern, ILogString
{
  public bool Matches(UnitId unitId) => unitId.Tag is Tag;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsServiceUnit);

  [DebuggerStepThrough]
  public string ToHoconString() => ToString();
}