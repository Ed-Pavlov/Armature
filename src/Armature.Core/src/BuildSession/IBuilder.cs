using System.Collections.Generic;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature.Core;

public interface IBuilder
{
  /// <summary>
  /// Builds a unit identified by the <paramref name="unitId"/>.
  /// </summary>
  /// <param name="unitId">The id of the unit to be built.</param>
  /// <param name="auxBuildStackPatternTree">Optional tree of build stack patterns which will be used along with
  /// the permanent one to build a unit or its dependencies.</param>
  /// <param name="engageParentBuilders">Determines whether to build a unit via parent builders in case it's not built in the scope of
  /// the current Builder.
  /// Default: true. </param>
  /// <returns></returns>
  BuildResult BuildUnit(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree, bool engageParentBuilders = true);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> by all build actions in spite of matching weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">The id of the unit to build.</param>
  /// <param name="auxBuildStackPatternTree">Additional build stack pattern tree containing build actions to build a unit or its dependencies.</param>
  /// <param name="engageParentBuilders">Determines whether to build a unit via parent builders in case it's not built in the scope of
  /// the current Builder.
  /// Default: true. </param>
  /// <returns>Returns <see cref="Empty{BuildResult}.List"/> if no units were built. </returns>
  List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, IBuildStackPattern? auxBuildStackPatternTree = null, bool engageParentBuilders = true);

  string Name { get; }
}
