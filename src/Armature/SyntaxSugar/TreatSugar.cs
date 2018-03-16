using System;
using Armature.Core;
using Armature.Extensibility;
using Armature.Framework;
using Armature.Framework.BuildActions;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatSugar<T> : AdjusterSugar
  {
    public TreatSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) {}

    /// <summary>
    ///   Treat Unit as is w/o any redirections
    /// </summary>
    public AdjusterSugar AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new AdjusterSugar(UnitSequenceMatcher);
    }

    /// <summary>
    ///   Pass the <see cref="instance" /> to any consumer of an Unit
    /// </summary>
    public void AsInstance([CanBeNull] T instance) => UnitSequenceMatcher.AddBuildAction(BuildStage.Cache, new SingletonBuildAction(instance));

    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreationBuildStep.Yes" /> adds a build step
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<see name="TRedirect" />, null)
    ///   as a creation build step.
    /// </param>
    public AdjusterSugar As<TRedirect>(AddCreationBuildStep addDefaultCreateAction) => As<TRedirect>(null, addDefaultCreateAction);

    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreationBuildStep.Yes" /> adds a build step
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<see name="TRedirect" />, <see cref="token" />)
    ///   as a creation build step.
    /// </param>
    /// <typeparam name="TRedirect"></typeparam>
    public AdjusterSugar As<TRedirect>(object token = null, AddCreationBuildStep addDefaultCreateAction = AddCreationBuildStep.Yes)
    {
      var redirectTo = typeof(TRedirect);

      //Todo: should this check be moved inside RedirectTypeBuildAction?
      if (!typeof(T).IsAssignableFrom(redirectTo))
        throw new Exception("Not assignable");

      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(redirectTo, token));

      var nextBuildStep = UnitSequenceMatcher;
      if (addDefaultCreateAction == AddCreationBuildStep.Yes)
      {
        nextBuildStep = new WeakUnitSequenceMatcher(Match.Type<TRedirect>(token), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);

        UnitSequenceMatcher
          .AddOrGetUnitMatcher(nextBuildStep)
          .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      }

      return new AdjusterSugar(nextBuildStep);
    }

    public CreateSugar<T> Created<TRedirect>(object token = null)
    {
      var redirectTo = typeof(TRedirect);

      //Todo: should this check be moved inside RedirectTypeBuildAction?
      if (!typeof(T).IsAssignableFrom(redirectTo))
        throw new Exception("Not assignable");

      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(redirectTo, token));
      return new CreateSugar<T>(UnitSequenceMatcher, token);
    }
    
    public void CreatedBy([NotNull] Func<IBuildSession, T> factoryMethod) =>
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T>(factoryMethod));

    public AdjusterSugar CreatedBy<T1>([NotNull] Func<IBuildSession, T1, T> factoryMethod) => 
      new AdjusterSugar(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod)));

    public AdjusterSugar CreatedBy<T1, T2>([NotNull] Func<IBuildSession, T1, T2, T> factoryMethod) =>
      new AdjusterSugar(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    public AdjusterSugar CreatedBy<T1, T2, T3>([NotNull] Func<IBuildSession, T1, T2, T3, T> factoryMethod) => 
      new AdjusterSugar(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));
 }
}