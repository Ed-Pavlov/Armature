using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  public interface IBuildPlansCollection
  {
    /// <summary>
    /// Returns all matched build actions for <paramref name="buildSequence" /> where the last <see cref="UnitInfo"/> is a currently building Unit.
    /// <see cref="IBuildStep.GetBuildActions"/>
    /// </summary>
    /// <param name="buildSequence">Current build sequence, the last item is a currently building Unit, all predecessor is a context of building session</param>
    /// <returns>Returns a collections of weighted build actions grouped by bulding stage</returns>
    MatchedBuildActions GetActions(IList<UnitInfo> buildSequence);

    /// <summary>
    /// Adds a <see cref="IBuildStep"/> into build plans collection. <paramref name="buildStep"/> with all its children represents one or more
    /// build plans. Also build plan can be completed later, <see cref="GetBuildStep"/> for details.
    /// </summary>
    IBuildPlansCollection AddBuildStep([NotNull] IBuildStep buildStep);

    /// <summary>
    /// Matches a whole sequence and returns the last <see cref="IBuildStep"/> if found. Used to create build plans
    /// <see cref="IBuildStep.GetChildBuldStep"/>
    /// </summary>
    [CanBeNull]
    IBuildStep GetBuildStep(ArrayTail<IBuildStep> buildStepsSequence);
  }
}