using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents a build step which triggers on an <see cref="UnitInfo"/> and contains build actions which should be performed to build a unit.
  /// </summary>
  public interface IBuildStep : IEquatable<IBuildStep>
  {
    /// <summary>
    /// Returns all matched build actions for <paramref name="buildSequence" /> where the last <see cref="UnitInfo"/> is a currently building Unit.
    /// </summary>
    /// <param name="inputWeight">Weight of matching, it increases during passing by children <see cref="IBuildStep"/></param>
    /// <param name="buildSequence">Current build sequence, the last item is a currently building Unit, all predecessor is a context of the building session.</param>
    /// <returns>Returns a collections of weighted build actions grouped by bulding stage</returns>
    [CanBeNull]
    MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence);

    /// <summary>
    /// Adds a build step into the build plan representing by the sequence of build steps from the root to "this" build step.
    /// </summary>
    void AddBuildStep([NotNull] IBuildStep buildStep);
    
    /// <summary>
    /// Removes a build step from the build plan representing by the sequence of build steps from the root to "this" build step. 
    /// </summary>
    /// <param name="buildStep"></param>
    /// <returns>Returns true if build step was remvoed or false if there was no such build step in the collection.</returns>
    bool RemoveBuildStep([NotNull] IBuildStep buildStep);

    /// <summary>
    /// Build step with its children and their chidlrens represents a tree where the one branch is a one build plan building a unit.
    /// </summary>
    [NotNull]
    IEnumerable<IBuildStep> Children { get; }
  }
}