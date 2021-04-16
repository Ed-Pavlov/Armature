namespace Armature.Core
{
  /// <summary>
  ///   Represents a matcher which matches the unit with a pattern.
  /// </summary>
  /// <remarks>
  ///   Unlike <see cref="IQuery" /> which works on unit sequence at whole, this matcher matches one unit id.
  ///   Reuses logic of matching one unit in different <see cref="IQuery" />
  /// </remarks>
  public interface IUnitIdMatcher
  {
    /// <summary> Checks if passed <paramref name="unitId"/> matches the pattern </summary>
    bool Matches(UnitId unitId);
  }
}
