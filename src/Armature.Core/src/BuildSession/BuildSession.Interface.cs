using System;
using System.Collections.Generic;
using Armature.Core.Sdk;

namespace Armature.Core;

public partial class BuildSession
{
  /// <inheritdoc />
  private class Interface : IBuildSession
  {
    private readonly BuildSession _buildSession;

    public Interface(BuildSession buildSession, Stack stack)
    {
      Stack    = stack;
      _buildSession = buildSession ?? throw new ArgumentNullException(nameof(buildSession));
    }

    ///<inheritdoc />
    public BuildResult BuildResult { get; set; }

    ///<inheritdoc />
    public Stack Stack { get; }

    ///<inheritdoc />
    public BuildResult BuildUnit(UnitId unitId) => _buildSession.BuildUnit(unitId);

    ///<inheritdoc />
    public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId) => _buildSession.BuildAllUnits(unitId);
  }
}