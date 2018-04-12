using System.Collections.Generic;
using Resharper.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Inteface of build session to pass into <see cref="IBuildAction"/>
  /// </summary>
  public interface IBuildSession
  {
    /// <summary>
    /// The result of building. Build actions can check if the unit is already built, or set the result.
    /// </summary>
    BuildResult BuildResult { get; set; }
    
    /// <summary>
    /// The sequence of units representing a build session, the last one is the unit under construction,
    /// the previous are the context of the build session. Each next unit info is the dependency of the previous one.
    /// </summary>
    IEnumerable<UnitInfo> BuildSequence { get; }
    
    /// <summary>
    /// Builds a unit represented by <see cref="UnitInfo" /> in the context of the current build session
    /// </summary>
    /// <returns>Returns an instance or null if unit can't be built.</returns>
    BuildResult BuildUnit([NotNull] UnitInfo unitInfo);
    
    /// <summary>
    /// Builds all units represented by <see cref="UnitInfo" /> in the context of the current build session
    /// </summary>
    /// <returns>Returns an instance or null if unit can't be built.</returns>
    IReadOnlyList<BuildResult> BuildAllUnits([NotNull] UnitInfo unitInfo);
  }
}