using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// This is an restricted interface of the <see cref="BuildSession" /> passed to <see cref="IBuildAction.Process" /> and <see cref="IBuildAction.PostProcess" />
  /// </summary>
  public class UnitBuilder : IBuildSession
  {
    private readonly BuildSession _buildSession;

    public UnitBuilder([NotNull] IEnumerable<UnitInfo> buildSequence, [NotNull] BuildSession buildSession)
    {
      if (buildSequence is null) throw new ArgumentNullException(nameof(buildSequence));
      if (buildSession is null) throw new ArgumentNullException(nameof(buildSession));

      _buildSession = buildSession;
      BuildSequence = buildSequence;
    }

    /// <summary>
    ///   The result of building. Build actions can check if the unit is already built, or set the unit.
    /// </summary>
    public BuildResult BuildResult { get; set; }
    
    /// <summary>
    ///   The sequence of units representing a build session, the last one is the unit to be built,
    ///   the previous are the context of the build session. Each next unit info is the dependency of the previous one.
    /// </summary>
    public IEnumerable<UnitInfo> BuildSequence { get; }
    
    /// <summary>
    ///   Builds a unit represented by <see cref="UnitInfo" /> in the context of the current build session
    /// </summary>
    /// <returns>Returns an instance or null if null is registered as an unit.</returns>
    [CanBeNull]
    public BuildResult BuildUnit([NotNull] UnitInfo unitInfo) => _buildSession.BuildUnit(unitInfo);
  }
}