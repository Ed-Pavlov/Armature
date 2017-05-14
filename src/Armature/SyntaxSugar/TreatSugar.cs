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

    public AdjusterSugar AsIs()
    { 
      _buildStep.AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new AdjusterSugar(_buildStep);
    }

    public void AsInstance([CanBeNull] T instance)
    {
      _buildStep.AddBuildAction(BuildStage.Cache, new SingletonBuildAction(instance));
    }

    /// <summary>
    /// Overload to call As with set <param name="addDefaultCreateAction"/> but w/o a token. Bool is not suitable because bool value can be passed as a token
    /// </summary>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for {<typeparamref name="TRedirect"/>, null} pair
    /// as a creation build step.</param>
    public AdjusterSugar As<TRedirect>(AddCreationBuildStep addDefaultCreateAction)
    {
      return As<TRedirect>(null, addDefaultCreateAction);
    }

    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">If <see cref="AddCreationBuildStep.Yes"/> adds a build step
    /// <see cref="Default.CreationBuildAction"/> for {<typeparamref name="TRedirect"/>, <paramref name="token"/>} pair
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
        nextBuildStep = new WeakBuildSequenceBuildStep(Match.Type<TRedirect>(token));
        nextBuildStep.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
        _buildStep.AddBuildStep(nextBuildStep);
      }

      return new AdjusterSugar(nextBuildStep);
    }

    public void CreatedBy([NotNull] Func<Build.Session, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T>(factoryMethod));
    }

    public void CreatedBy<T1>([NotNull] Func<Build.Session, T1, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod));
    }

    public void CreatedBy<T1, T2>([NotNull] Func<Build.Session, T1, T2, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod));
    }

    public void CreatedBy<T1, T2, T3>([NotNull] Func<Build.Session, T1, T2, T3, T> factoryMethod)
    {
      _buildStep.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod));
    }
  }
}