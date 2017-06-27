using System;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatSugar<T> : AdjusterSugar
  {
    private readonly StaticBuildStep _buildStep;

    public TreatSugar(StaticBuildStep buildStep) : base(buildStep)
    {
      _buildStep = buildStep;
    }

    /// <summary>
    /// Treat Unit as is w/o any redirections
    /// </summary>
    public AdjusterSugar AsIs()
    { 
      _buildStep.AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new AdjusterSugar(_buildStep);
    }

    /// <summary>
    /// Pass the <see cref="instance"/> to any consumer of an Unit 
    /// </summary>
    public void AsInstance([CanBeNull] T instance)
    {
      _buildStep.AddBuildAction(BuildStage.Cache, new SingletonBuildAction(instance));
    }

    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for <see cref="UnitInfo"/>(<see name="TRedirect"/>, null)
    /// as a creation build step.</param>
    public AdjusterSugar As<TRedirect>(AddCreationBuildStep addDefaultCreateAction)
    {
      return As<TRedirect>(null, addDefaultCreateAction);
    }

    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for <see cref="UnitInfo"/>(<see name="TRedirect"/>, <see cref="token"/>)
    /// as a creation build step.</param>
    /// <typeparam name="TRedirect"></typeparam>
    public AdjusterSugar As<TRedirect>(object token = null, AddCreationBuildStep addDefaultCreateAction = AddCreationBuildStep.Yes)
    {
      var redirectTo = typeof(TRedirect);

      //Todo: should this check be moved inside RedirectTypeBuildAction?
      if(!typeof(T).IsAssignableFrom(redirectTo))
        throw new Exception("Not assignable");

      _buildStep.AddBuildAction(BuildStage.Redirect, new RedirectTypeBuildAction(redirectTo, token));

      var nextBuildStep = _buildStep;
      if (addDefaultCreateAction == AddCreationBuildStep.Yes)
      {
        nextBuildStep = new UnitSequenceWeakMatchingBuildStep(Match.Type<TRedirect>(token));
        nextBuildStep.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
        _buildStep.AddBuildStep(nextBuildStep);
      }

      return new AdjusterSugar(nextBuildStep);
    }

    public void CreatedBy([NotNull] Func<UnitBuilder, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T>(factoryMethod));
    }

    public void CreatedBy<T1>([NotNull] Func<UnitBuilder, T1, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod));
    }

    public void CreatedBy<T1, T2>([NotNull] Func<UnitBuilder, T1, T2, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod));
    }

    public void CreatedBy<T1, T2, T3>([NotNull] Func<UnitBuilder, T1, T2, T3, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod));
    }
  }
}