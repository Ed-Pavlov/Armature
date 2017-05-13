using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatOpenGenericSugar
  {
    private readonly BuildStep _buildStep;

    public TreatOpenGenericSugar([NotNull] BuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");

      _buildStep = buildStep;
    }

    /// <summary>
    /// Overload to call As with set <param name="addDefaultCreateAction"/> but w/o a token. Bool is not suitable because bool value can be passed as a token
    /// </summary>
    /// <param name="openGenericType"></param>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for {<paramref name="openGenericType"/>, null} pair
    /// as a creation build step.</param>
    public AdjusterSugar As(Type openGenericType, AddCreationBuildStep addDefaultCreateAction)
    {
      return As(openGenericType, null, addDefaultCreateAction);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="openGenericType"></param>
    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for {<paramref name="openGenericType"/>, <paramref name="token"/>} pair
    /// as a creation build step.</param>
    public AdjusterSugar As(Type openGenericType, object token = null, AddCreationBuildStep addDefaultCreateAction = AddCreationBuildStep.Yes)
    {
      _buildStep.AddBuildAction(BuildStage.Redirect, new RedirectOpenGenericTypeBuildAction(openGenericType, token));

      var nextBuildStep = _buildStep;
      if (addDefaultCreateAction == AddCreationBuildStep.Yes)
      {
        nextBuildStep = new WeakBuildSequenceBuildStep(Match.OpenGenericType(openGenericType, token));
        nextBuildStep.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
        _buildStep.AddChildBuildStep(nextBuildStep);
      }

      return new AdjusterSugar(nextBuildStep);
    }
  }
}