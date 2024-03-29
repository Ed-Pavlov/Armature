﻿using System;
using System.Collections.Generic;

namespace Armature.Core;

public partial class BuildSession
{
  /// <summary>
  /// This is an restricted interface of the <see cref="BuildSession" /> passed
  /// to <see cref="IBuildAction.Process" /> and <see cref="IBuildAction.PostProcess" />
  /// </summary>
  private class Interface : IBuildSession
  {
    private readonly BuildSession _buildSession;

    public Interface(BuildSession buildSession, BuildChain buildChain)
    {
      BuildChain    = buildChain;
      _buildSession = buildSession ?? throw new ArgumentNullException(nameof(buildSession));
    }

    ///<inheritdoc />
    public BuildResult BuildResult { get; set; }

    ///<inheritdoc />
    public BuildChain BuildChain { get; }

    ///<inheritdoc />
    public BuildResult BuildUnit(UnitId unitId) => _buildSession.BuildUnit(unitId);

    ///<inheritdoc />
    public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => _buildSession.BuildAllUnits(unitId);
  }
}