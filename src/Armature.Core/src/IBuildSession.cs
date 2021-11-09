using System.Collections.Generic;


namespace Armature.Core;

/// <summary>
///   Interface of build session to pass into <see cref="IBuildAction" />
/// </summary>
public interface IBuildSession
{
  /// <summary>
  ///   The result of building. Build actions can check if the unit is already built, or set the result.
  /// </summary>
  BuildResult BuildResult { get; set; }

  /// <summary>
  ///   The sequence of units representing a build session, the last one is the unit under construction,
  ///   the previous are the context of the build session. Each next unit info is the dependency of the previous one.
  /// </summary>
  IEnumerable<UnitId> BuildSequence { get; }

  /// <summary>
  ///   Builds a unit represented by <see cref="UnitId" /> in the context of the current build session
  /// </summary>
  BuildResult BuildUnit(UnitId unitId);

  /// <summary>
  ///   Builds all units represented by <see cref="UnitId" /> in the context of the current build session
  /// </summary>
  /// <returns>Returns an instance or null if unit can't be built.</returns>
  List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId);
}