namespace BeatyBit.Armature.Core;

/// <summary>
/// Represents a pattern used to match with the unit id to gather a set of <see cref="IBuildAction"/> needed to build a unit.
/// </summary>
/// <remarks>
/// Unlike <see cref="IBuildStackPattern" /> which works on a build stack at whole, this pattern is used to match a single Unit.
/// </remarks>
public interface IUnitPattern
{
  /// <summary> Checks if passed <paramref name="unitId"/> matches the pattern.</summary>
  bool Matches(UnitId unitId);
}