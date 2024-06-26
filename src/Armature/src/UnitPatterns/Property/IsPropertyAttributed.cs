﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Checks if a unit to be built is an argument for an object property marked with <see cref="InjectAttribute"/> attribute
/// with an optional <see cref="InjectAttribute" />.<see cref="InjectAttribute.Tag" />
/// </summary>
public record IsPropertyAttributed : IsInjectPointBase
{
  /// <inheritdoc />
  [DebuggerStepThrough]
  public IsPropertyAttributed(object? injectPointTag = null) : base(injectPointTag) { }

  protected override IEnumerable<InjectAttribute> GetAttributes(UnitId unitId)
    => unitId.Kind is PropertyInfo propertyInfo ? propertyInfo.GetCustomAttributes<InjectAttribute>() : Empty<InjectAttribute>.Array;
}