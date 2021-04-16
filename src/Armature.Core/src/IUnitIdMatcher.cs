namespace Armature.Core
{
  /// <summary>
  ///   Represents a matcher which matches the unit with a pattern.
  /// </summary>
  /// <remarks>
  ///   Unlike <see cref="IQuery" /> which represent a logic how the units sequence is treated, this matcher matches one unit.
  ///   Using to reuse logic of matching one unit in different <see cref="IQuery" />
  /// </remarks>
  public interface IUnitIdMatcher
  {
    /// <returns>True if an implemented logic matches specified <paramref name="unitId"/></returns>
    bool Matches(UnitId unitId);
  }
}
