﻿using System.Diagnostics;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Checks if a unit is the list of properties of a type to inject dependencies.
/// </summary>
public record IsPropertyList : IUnitPattern
{
  public bool Matches(UnitId unitId) => unitId.Tag == SpecialTag.PropertyList && unitId.GetUnitTypeSafe() is not null;

  [DebuggerStepThrough]
  public override string ToString() => nameof(IsPropertyList);
}
