using System;
using System.Diagnostics;

namespace Armature.Core
{
  public static class UnitInfoExtension
  {
    /// <summary>
    /// Returns a <see cref="Type" /> if <see cref="UnitInfo.Id" /> is a type, otherwise throws exception.
    /// </summary>
    [DebuggerStepThrough]
    public static Type GetUnitType(this UnitInfo unitInfo) => (Type)unitInfo.Id! ?? throw new ArgumentException("UnitInfo id is not a type");

    /// <summary>
    /// Returns a <see cref="Type" /> if <see cref="UnitInfo.Id" /> is a type, otherwise null.
    /// </summary>
    [DebuggerStepThrough]
    public static Type? GetUnitTypeSafe(this UnitInfo unitInfo) => unitInfo.Id as Type;
  }
}