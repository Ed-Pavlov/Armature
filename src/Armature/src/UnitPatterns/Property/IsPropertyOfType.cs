﻿using System;
using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit to be built is an argument to inject into a property requires argument of the specified type.
/// </summary>
public record IsPropertyOfType : InjectPointOfTypeBase
{
  [DebuggerStepThrough]
  public IsPropertyOfType(IUnitPattern typePattern) : base(typePattern) { }

  protected override Type? GetInjectPointType(UnitId unitId) => (unitId.Kind as PropertyInfo)?.PropertyType;
}