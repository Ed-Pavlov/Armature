using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents a matcher which matches the sequence of building units with a pattern  
  /// </summary>
  public interface IUnitSequenceMatcher : IEquatable<IUnitSequenceMatcher>
  {
    /// <summary>
    /// The collection of all child matchers, used to find existing one, add new, or replace one with another
    /// </summary>
    [NotNull]
    ICollection<IUnitSequenceMatcher> Children { get; }

    /// <summary>
    /// Returns build actions for building unit if matches it
    /// </summary>
    /// <param name="buildingUnitsSequence">The sequence of units representing the context of currently building unit</param>
    /// <param name="inputMatchingWeight">The weight of matching which used by children matchers to calculate a final weight of matching</param>
    /// <returns>Returns a collections of weighted build actions grouped by a bulding stage</returns>
    [CanBeNull]
    MatchedBuildActions GetBuildActions(ArrayTail<UnitInfo> buildingUnitsSequence, int inputMatchingWeight);

    /// <summary>
    ///  Adds a build action factory which creates <see cref="IBuildAction"/> for a unit which is matched by this matcher
    /// </summary>
    /// <param name="buildStage">Build stage in which the build action is applied</param>
    /// <param name="buildAction">Build action</param>
    /// <param name="weight">The weight of the action, needed if several build actions are registered for the unit in one build stage</param>
    /// <returns>Returns 'this' in order to use fluent syntax</returns>
    IUnitSequenceMatcher AddBuildAction(object buildStage, IBuildAction buildAction, int weight);
  }
}