using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature.Sdk;

public static class UnitIdExtension
{
  /// <summary>
  /// Returns a <see cref="Type" /> if <see cref="UnitId.Kind" /> is a type, otherwise throws exception.
  /// </summary>
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Type GetUnitType(this UnitId unitId)
    => unitId.Kind as Type
    ?? throw new ArmatureException($"Unit {nameof(UnitId.Kind)} is not an instance of the class {nameof(Type)}")
        .AddData(nameof(unitId), unitId);

  /// <summary>
  /// Returns a <see cref="Type" /> if <see cref="UnitId.Kind" /> is a type, otherwise null.
  /// </summary>
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Type? GetUnitTypeSafe(this UnitId unitId) => unitId.Kind as Type;
}