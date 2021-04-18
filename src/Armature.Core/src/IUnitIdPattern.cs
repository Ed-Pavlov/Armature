namespace Armature.Core
{
  /// <summary>
  ///   Represents a matcher which matches the unit with a pattern.
  /// </summary>
  /// <remarks>
  ///   Unlike <see cref="IPatternTreeNode" /> which works on unit sequence at whole, this matcher matches one unit id.
  ///   Reuses logic of matching one unit in different <see cref="IPatternTreeNode" />
  /// </remarks>
  public interface IUnitIdPattern
  {
    /// <summary> Checks if passed <paramref name="unitId"/> matches the pattern </summary>
    bool Matches(UnitId unitId);
  }
}
