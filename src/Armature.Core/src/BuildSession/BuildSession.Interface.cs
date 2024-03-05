using System.Collections.Generic;
using Armature.Core.Sdk;

namespace Armature.Core;

public partial class BuildSession
{
  /// <inheritdoc />
  private class Interface(BuildSession buildSession, Stack stack) : IBuildSession
  {
    ///<inheritdoc />
    public BuildResult BuildResult { get; set; }

    ///<inheritdoc />
    public Stack Stack { get; } = stack;

    ///<inheritdoc />
    public BuildResult BuildUnit(UnitId unitId, bool engageParentBuilders = true) => buildSession.BuildUnit(unitId, engageParentBuilders);

    ///<inheritdoc />
    public List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, bool engageParentBuilders = true) => buildSession.BuildAllUnits(unitId, engageParentBuilders);
  }
}