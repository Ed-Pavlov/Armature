using System.Collections.Generic;
using Armature.Core.Sdk;

namespace Armature.Core;

public interface IBuilder
{
  /// <summary>
  /// Builds a unit identified by the <paramref name="unitId"/>.
  /// </summary>
  /// <param name="unitId">The id of the unit to be built.</param>
  /// <param name="auxBuildStackPatternTree">Optional tree of build stack patterns which will be used along with
  /// the permanent one to build a unit or it's dependencies.</param>
  /// <returns></returns>
  BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">The id of the unit to build.</param>
  /// <param name="auxBuildStackPatternTree">Additional build stack pattern tree containing build actions to build a unit or its dependencies.</param>
  /// <returns>Returns <see cref="Empty{BuildResult}.List"/> if no units were built. </returns>
  List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null);
}
