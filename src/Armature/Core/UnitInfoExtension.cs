using System;
using JetBrains.Annotations;

namespace Armature.Core
{
  public static class UnitInfoExtension
  {
    /// <summary>
    /// Returns a <see cref="Type"/> if <see cref="UnitInfo.Id"/> is a type, otherwise throws exception.
    /// </summary>
    [NotNull]
    public static Type GetUnitType([NotNull] this UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      return (Type)unitInfo.Id;
    }

    /// <summary>
    /// Returns a <see cref="Type"/> if <see cref="UnitInfo.Id"/> is a type, otherwise null.
    /// </summary>
    [CanBeNull]
    public static Type GetUnitTypeSafe([NotNull] this UnitInfo unitInfo)
    {
      if (unitInfo == null) throw new ArgumentNullException("unitInfo");
      return unitInfo.Id as Type;
    }
  }
}