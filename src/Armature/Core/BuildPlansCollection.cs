using System;
using System.Collections.Generic;
using Armature.Common;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// The collection of build plans. Build plan is a sequence of build steps for building a unit.
  /// All build plans are contained in a forest of trees form <see cref="IBuildStep"/> for details.
  /// </summary>
  public class BuildPlansCollection
  {
    /// <summary>
    /// Reuse implementation of <see cref="BuildStepBase"/> to implement <see cref="BuildPlansCollection"/> public interface
    /// </summary>
    private readonly RootBuildStep _rootBuildStep = new RootBuildStep();

    /// <summary>
    /// Returns build actions which should be performed to build a unit represented by the last item in <paramref name="buildSequence"/>
    /// </summary>
    /// <param name="buildSequence">The sequence of units info, the last one is currently building, the previous are the context of
    /// the build session. </param>
    /// <remarks>If there is type A which depends on class B, during building A, B should be built and build sequence will be
    /// [A, B] in this case.</remarks>
    /// <returns>Returns all matches build actions for <see cref="buildSequence"/>. All actions are grouped by building stage
    /// and has a "weight of matching". <see cref="MatchedBuildActions"/> type declaration for details.</returns>
    public MatchedBuildActions GetBuildActions([NotNull] IList<UnitInfo> buildSequence)
    {
      if (buildSequence == null) throw new ArgumentNullException("buildSequence");
      return _rootBuildStep.GetBuildActions(0, buildSequence.GetTail(0));
    }

    /// <summary>
    /// Adds a root build step (tree) into the forest of trees 
    /// </summary>
    /// <param name="buildStep"></param>
    /// <exception cref="ArgumentNullException"></exception>
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

    private class RootBuildStep : BuildStepBase
    {
      public override MatchedBuildActions GetBuildActions(int inputWeight, ArrayTail<UnitInfo> buildSequence)
      {
        return GetChildrenActions(inputWeight, buildSequence);
      }

      public override bool Equals(IBuildStep other)
      {
        throw new NotSupportedException();
      }
    }
  }
}