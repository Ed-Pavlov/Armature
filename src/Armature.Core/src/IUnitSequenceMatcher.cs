using System;
using System.Collections.Generic;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Represents a matcher which matches the sequence of building units with a pattern
  /// </summary>
  public interface IUnitSequenceMatcher : IEquatable<IUnitSequenceMatcher>, ILogable
  {
    /// <summary>
    ///   The collection of all child matchers, used to find existing one, add new, or replace one with another
    /// </summary>
    ICollection<IUnitSequenceMatcher> Children { get; }

    /// <summary>
    ///   Returns build actions which should be performed to build a unit represented by the last item of <paramref name="buildingUnitsSequence" />
    /// </summary>
    /// <param name="buildingUnitsSequence">
    ///   The sequence of units representing a build session, the last one is the unit to be built,
    ///   the previous are the context of the build session. Each next unit info is the dependency of the previous one.
    /// </param>
    /// <param name="inputWeight">
    ///   The weight of matching which used by children matchers to calculate a final weight of matching
    ///   Not applicable to BuildPlansCollection in common case
    /// </param>
    /// <remarks>
    ///   If there is type A which depends on class B, during building A, B should be built and build sequence will be
    ///   [A, B] in this case.
    /// </remarks>
    /// <returns>
    ///   Returns all matched build actions for the <paramref name="buildingUnitsSequence" />. All actions are grouped by a building stage
    ///   and coupled with a "weight of matching". See <see cref="MatchedBuildActions" /> type declaration for details.
    /// </returns>
    MatchedBuildActions? GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputWeight);

    /// <summary>
    ///   Adds a <see cref="IBuildAction" /> for a unit which is matched by this matcher
    /// </summary>
    /// <param name="buildStage">Build stage in which the build action is applied</param>
    /// <param name="buildAction">Build action</param>
    /// <returns>Returns 'this' in order to use fluent syntax</returns>
    IUnitSequenceMatcher AddBuildAction(object buildStage, IBuildAction buildAction);
  }
}