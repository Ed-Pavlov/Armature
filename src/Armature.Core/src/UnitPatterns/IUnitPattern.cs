namespace Armature.Core;

/// <summary>
/// Represents a pattern used to match with the unit id in order to gather a set of <see cref="IBuildAction"/> needed to build a unit.
/// </summary>
/// <remarks>
/// Unlike <see cref="IBuildChainPattern" /> which works on a build chain at whole, this pattern is used to match one unit id.
/// Reuses logic of matching one unit in different <see cref="IBuildChainPattern" />
/// </remarks>
public interface IUnitPattern
{
  /// <summary> Checks if passed <paramref name="unitId"/> matches the pattern </summary>
  bool Matches(UnitId unitId);
}