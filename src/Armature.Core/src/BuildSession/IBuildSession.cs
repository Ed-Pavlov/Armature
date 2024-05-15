using System.Collections.Generic;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature.Core;

/// <summary>
/// This is a limited interface of the <see cref="BuildSession" /> passed to <see cref="IBuildAction.Process" />
/// and <see cref="IBuildAction.PostProcess" />.
/// </summary>
public interface IBuildSession
{
  /// <summary>
  /// The result of building. Build actions can check if the unit is already built, or set the result.
  /// </summary>
  BuildResult BuildResult { get; set; }

  /// <summary>
  /// The stack of units representing a build session. See <see cref="BuildSession.Stack"/> for details.
  /// </summary>
  BuildSession.Stack Stack { get; }

  /// <summary>
  /// Builds a unit represented by <see cref="UnitId" /> in the context of the current build session.
  /// </summary>
  /// <param name="unitId">"Id" of the unit to build. See <see cref="IBuildStackPattern" /> for details</param>
  /// <param name="engageParentBuilders">Determines whether to build a unit via parent builders in case it's not built in the scope of
  /// the current build session. Default: true. </param>
  BuildResult BuildUnit(UnitId unitId, bool engageParentBuilders = true);

  /// <summary>
  /// Builds all units represented by <see cref="UnitId" /> with all build actions in spite of the weight.
  /// This can be useful to build all implementers of an interface.
  /// </summary>
  /// <param name="unitId">"Id" of the unit to build. See <see cref="IBuildStackPattern" /> for details</param>
  /// <param name="engageParentBuilders">If true <see cref="Builder.BuildAllUnits"/> is called for whole parent builders graph,
  /// if false build unit only in the scope of the current build sesstion. Default: true.</param>
  /// <returns>Returns an empty list if no units were built.</returns>
  List<Weighted<BuildResult>> BuildAllUnits(UnitId unitId, bool engageParentBuilders = true);
}
