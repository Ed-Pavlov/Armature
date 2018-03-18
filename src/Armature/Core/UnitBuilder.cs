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

    ///<inheritdoc />
    public BuildResult BuildResult { get; set; }
    
    ///<inheritdoc />
    public IEnumerable<UnitInfo> BuildSequence { get; }
    
    ///<inheritdoc />
    [CanBeNull]
    public BuildResult BuildUnit([NotNull] UnitInfo unitInfo) => _buildSession.BuildUnit(unitInfo);

    ///<inheritdoc />
    public IReadOnlyList<BuildResult> BuildAllUnits(UnitInfo unitInfo) => _buildSession.BuildAllUnits(unitInfo);
  }
}