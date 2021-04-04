using System;
using System.Collections.Generic;


namespace Armature.Core
{
  public partial class BuildSession
  {
    /// <summary>
    ///   This is an restricted interface of the <see cref="BuildSession" /> passed to <see cref="IBuildAction.Process" /> and <see cref="IBuildAction.PostProcess" />
    /// </summary>
    private class Interface : IBuildSession
    {
      private readonly BuildSession _buildSession;

      public Interface(IEnumerable<UnitInfo> buildSequence, BuildSession buildSession)
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
      public BuildResult BuildUnit(UnitInfo unitInfo) => _buildSession.BuildUnit(unitInfo);

      ///<inheritdoc />
      public IReadOnlyList<BuildResult>? BuildAllUnits(UnitInfo unitInfo) => _buildSession.BuildAllUnits(unitInfo);
    }
  }
}