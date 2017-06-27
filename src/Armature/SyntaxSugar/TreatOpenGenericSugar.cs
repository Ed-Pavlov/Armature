using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatOpenGenericSugar
  {
    private readonly StaticBuildStep _buildStep;

    public TreatOpenGenericSugar([NotNull] StaticBuildStep buildStep)
    {
      if (buildStep == null) throw new ArgumentNullException("buildStep");

      _buildStep = buildStep;
    }

    /// <param name="openGenericType"></param>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for <see cref="UnitInfo"/>(<paramref name="openGenericType"/>, null) as a creation build step.</param>
    public AdjusterSugar As(Type openGenericType, AddCreationBuildStep addDefaultCreateAction)
    {
      return As(openGenericType, null, addDefaultCreateAction);
    }

    /// <param name="openGenericType"></param>
    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for <see cref="UnitInfo"/>(<paramref name="openGenericType"/>, <see cref="token"/>) 
    /// as a creation build step.</param>
    public AdjusterSugar As(Type openGenericType, object token = null, AddCreationBuildStep addDefaultCreateAction = AddCreationBuildStep.Yes)
    {
      _buildStep.AddBuildAction(BuildStage.Redirect, new RedirectOpenGenericTypeBuildAction(openGenericType, token));

      var nextBuildStep = _buildStep;
      if (addDefaultCreateAction == AddCreationBuildStep.Yes)
      {
        nextBuildStep = new UnitSequenceWeakMatchingBuildStep(Match.OpenGenericType(openGenericType, token));
        nextBuildStep.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
        _buildStep.AddBuildStep(nextBuildStep);
      }

      return new AdjusterSugar(nextBuildStep);
    }
  }
}