using System;
using System.Diagnostics;

namespace Armature.Core.Sdk
{
  public static class UnitIdExtension
  {
    /// <summary>
    ///   Returns a <see cref="Type" /> if <see cref="UnitId.Kind" /> is a type, otherwise throws exception.
    /// </summary>
    [DebuggerStepThrough]
    public static Type GetUnitType(this UnitId unitId) => (Type) unitId.Kind! ?? throw new ArgumentNullException(nameof(unitId.Kind));

    /// <summary>
    ///   Returns a <see cref="Type" /> if <see cref="UnitId.Kind" /> is a type, otherwise null.
    /// </summary>
    [DebuggerStepThrough]
    public static Type? GetUnitTypeSafe(this UnitId unitId) => unitId.Kind as Type;
  }
}
