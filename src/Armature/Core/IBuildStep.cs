using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Represents a build step which triggers on an <see cref="UnitInfo"/>
  /// </summary>
  public interface IBuildStep : IEquatable<IBuildStep>
  {
    /// <summary>
    /// Returns all matched build actions for <paramref name="buildSequence" /> where the last <see cref="UnitInfo"/> is a currently building Unit.
    /// </summary>
    /// <param name="inputWeight">Weight of matching, it increases during passing by children <see cref="IBuildStep"/></param>
    /// <param name="buildSequence">Current build sequence, the last item is a currently building Unit, all predecessor is a context of building session</param>
    /// <returns>Returns a collections of weighted build actions grouped by bulding stage</returns>
    [CanBeNull]
    MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence);

    void AddBuildStep([NotNull] IBuildStep buildStep);

    [NotNull]
    IEnumerable<IBuildStep> Children { get; }
  }
}