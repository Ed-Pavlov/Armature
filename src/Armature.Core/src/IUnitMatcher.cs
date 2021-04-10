namespace Armature.Core
{
  /// <summary>
  ///   Represents a matcher which matches the unit with a pattern.
  /// </summary>
  /// <remarks>
  ///   Unlike <see cref="IUnitSequenceMatcher" /> which represent a logic how the units sequence is treated, this matcher matches one unit.
  ///   Using to reuse logic of matching one unit in different <see cref="IUnitSequenceMatcher" />
  /// </remarks>
  public interface IUnitMatcher
  {
    bool Matches(UnitInfo unitInfo);
  }
}
