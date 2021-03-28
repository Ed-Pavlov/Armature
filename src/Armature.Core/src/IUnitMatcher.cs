using System;

namespace Armature.Core
{
  /// <summary>
  ///  Represents a pattern which can be matched with an unit info.
  /// </summary>
  /// <remarks>
  ///  Unlike <see cref="IUnitSequenceMatcher" /> which represent a logic how the units sequence is treated, this matcher matches one unit.
  ///  Is used to reuse logic of matching one unit in different <see cref="IUnitSequenceMatcher" />
  /// </remarks>
  public interface IUnitMatcher
  {
    bool Matches(UnitInfo unitInfo);
  }
}