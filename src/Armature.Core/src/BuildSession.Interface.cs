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
        _buildSession = buildSession ?? throw new ArgumentNullException(nameof(buildSession));
        BuildSequence = buildSequence ?? throw new ArgumentNullException(nameof(buildSequence));
      }

      ///<inheritdoc />
      /// Don't cache the value of this property during a build session, because any BuildAction able to change it at Process and PostProcess
      public BuildResult? BuildResult { get; set; }

      ///<inheritdoc />
      public IEnumerable<UnitInfo> BuildSequence { get; }

      ///<inheritdoc />
      public BuildResult? BuildUnit(UnitInfo unitInfo) => _buildSession.BuildUnit(unitInfo);

      ///<inheritdoc />
      public IReadOnlyList<BuildResult>? BuildAllUnits(UnitInfo unitInfo) => _buildSession.BuildAllUnits(unitInfo);
    }
  }
}