using System;
using System.Collections.Generic;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  ///   Represents a query applied to a sequence of building units in order to find build actions needed to build a unit
  /// </summary>
  public interface IPatternTreeNode : IEquatable<IPatternTreeNode>, ILogable
  {
    /// <summary>
    ///   The collection of all child sub queries, used to find existing one, add new, or replace one with another
    /// </summary>
    ICollection<IPatternTreeNode> Children { get; }

    /// <summary>
    ///   Returns build actions which should be performed to build a unit represented by the last item of <paramref name="unitSequence" />
    /// </summary>
    /// <param name="unitSequence">
    ///   The sequence of units representing a build session, the last one is the unit to be built,
    ///   the previous are the context of the build session. Each next unit is the dependency of the previous one.
    /// </param>
    /// <param name="inputWeight">
    ///   The weight of matching which used by children matchers to calculate a final weight of matching
    ///   Not applicable to BuildPlansCollection in common case
    /// </param>
    /// <remarks>
    ///   If there is type A which depends on class B, during building A, B should be built and build sequence will be [A, B] in this case.
    /// </remarks>
    /// <returns>
    ///   Returns all matched build actions for the <paramref name="unitSequence" />. All actions are grouped by a building stage
    ///   and coupled with a "weight of matching". See <see cref="BuildActionBag" /> for details.
    /// </returns>
    BuildActionBag? GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight);

    /// <summary>
    ///   Adds a <see cref="IBuildAction" /> for a "to be built" unit which is matched by this query
    /// </summary>
    /// <param name="buildStage">Build stage in which the build action is executed</param>
    /// <param name="buildAction">Build action</param>
    /// <returns>Returns 'this' in order to use fluent syntax</returns>
    IPatternTreeNode UseBuildAction(object buildStage, IBuildAction buildAction);
  }
}
