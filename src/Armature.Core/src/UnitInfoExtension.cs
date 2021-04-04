using System;
using System.Diagnostics;


namespace Armature.Core
{
  public static class UnitInfoExtension
  {
    /// <summary>
    ///   Returns a <see cref="Type" /> if <see cref="UnitInfo.Id" /> is a type, otherwise throws exception.
    /// </summary>
    [DebuggerStepThrough]
    public static Type GetUnitType(this UnitInfo unitInfo) =>
      (Type)unitInfo.Id! ?? throw new ArgumentNullException(nameof(unitInfo));

    /// <summary>
    ///   Returns a <see cref="Type" /> if <see cref="UnitInfo.Id" /> is a type, otherwise null.
    /// </summary>
    [DebuggerStepThrough]
    public static Type? GetUnitTypeSafe(this UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException(nameof(unitInfo));

      return unitInfo.Id as Type;
    }
  }
}