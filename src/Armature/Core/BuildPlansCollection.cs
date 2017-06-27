using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// The collection of build plans. Build plan is a sequence of build steps building an unit.
  /// All build plans are contained in a forest of trees form. See <see cref="IBuildStep"/> for details.
  /// </summary>
  public class BuildPlansCollection
  {
    private readonly RootBuildStep _rootBuildStep = new RootBuildStep();

    /// <summary>
    /// Returns build actions which should be performed to build an unit represented by the last item of <paramref name="buildSequence"/>
    /// </summary>
    /// <param name="buildSequence">The sequence of units representing a build session, the last one is the unit to be built, 
    /// the previous are the context of the build session. Each next unit info is the dependency of the previous one. </param>
    /// <remarks>If there is type A which depends on class B, during building A, B should be built and build sequence will be
    /// [A, B] in this case.</remarks>
    /// <returns>Returns all matched build actions for the <see cref="buildSequence"/>. All actions are grouped by a building stage
    /// and coupled with a "weight of matching". See <see cref="MatchedBuildActions"/> type declaration for details.</returns>
    public MatchedBuildActions GetBuildActions([NotNull] IList<UnitInfo> buildSequence)
    {
      if (buildSequence == null) throw new ArgumentNullException("buildSequence");
      return _rootBuildStep.GetBuildActions(0, buildSequence.GetTail(0));
    }

    /// <summary>
    /// Adds a root build step (tree) into the forest of trees 
    /// </summary>
    /// <param name="buildStep">The build step to add, it can have child build steps of can be filled with them later</param>
    public void AddBuildStep([NotNull] IBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");
      _rootBuildStep.AddBuildStep(buildStep);
    }

    /// <summary>
    /// Collection of root build steps
    /// </summary>
    public IEnumerable<IBuildStep> Children
    {
      get { return _rootBuildStep.Children; }
    }

    /// <summary>
    /// Reuse implementation of <see cref="BuildStepBase"/> to implement <see cref="BuildPlansCollection"/> public interface
    /// </summary>
    private class RootBuildStep : BuildStepBase
    {
      public override MatchedBuildActions GetBuildActions(int inputMatchingWeight, ArrayTail<UnitInfo> matchingPattern)
      {
        return GetChildrenActions(inputMatchingWeight, matchingPattern);
      }

      public override bool Equals(IBuildStep other)
      {
        throw new NotSupportedException();
      }
    }
  }
}