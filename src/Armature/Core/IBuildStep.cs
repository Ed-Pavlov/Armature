using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents a build step which matches with an <see cref="UnitInfo"/> and contains build actions which should be performed to build the unit.
  /// </summary>
  /// <remarks>Implementing <see cref="IEquatable{IBuildStep}"/> is needed to implement <see cref="RemoveBuildStep"/> method and also allows
  /// to traverse and add on a tree of build steps</remarks>
  public interface IBuildStep : IEquatable<IBuildStep>
  {
    /// <summary>
    /// Returns all build actions from the build step matched with <paramref name="matchingPattern" />.
    /// </summary>
    /// <param name="inputMatchingWeight">The weight of matching which used by children build steps to calculate a final weight of matching</param>
    /// <param name="matchingPattern">The sequence of unit infos to match with build steps and find suitable one</param>
    /// <returns>Returns a collections of weighted build actions grouped by a bulding stage</returns>
    [CanBeNull]
    MatchedBuildActions GetBuildActions(int inputMatchingWeight, ArrayTail<UnitInfo> matchingPattern);

    /// <summary>
    /// Adds child build step into the build plan representing by the sequence of build steps from the root to "this" build step.
    /// </summary>
    void AddBuildStep([NotNull] IBuildStep buildStep);
    
    /// <summary>
    /// Removes child build step from the build plan representing by the sequence of build steps from the root to "this" build step. 
    /// </summary>
    /// <returns>Returns true if build step was removed or false if there was no such build step in the collection.</returns>
    bool RemoveBuildStep([NotNull] IBuildStep buildStep);

    /// <summary>
    /// The collection of all child steps.
    /// </summary>
    [NotNull]
    IEnumerable<IBuildStep> Children { get; }
  }
}