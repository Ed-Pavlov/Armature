using System;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public class TreatingTuner<T> : Tuner
  {
    public TreatingTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) {}

    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object of type <typeparamref name="T"/>.
    /// Tune plan of building it by subsequence calls.  
    /// </summary>
    public Tuner AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new Tuner(UnitSequenceMatcher);
    }

    /// <summary>
    ///   For all who depends on <typeparamref name="T"/> inject <paramref name="instance"/>.
    /// </summary>
    public void AsInstance([CanBeNull] T instance) => UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new SingletonBuildAction(instance));

    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object of type <typeparamref name="TRedirect"/>.
    /// Tune plan of building it by subsequence calls.  
    /// </summary>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action <see cref="Default.CreationBuildAction" /> for
    ///   <see cref="UnitInfo" />(<see name="TRedirect" />, null) as a creation build action.
    /// </param>
    public Tuner As<TRedirect>(AddCreateBuildAction addDefaultCreateAction) where TRedirect : T => As<TRedirect>(null, addDefaultCreateAction);

    ///<inheritdoc cref="As{TRedirect}(Armature.AddCreateBuildAction)"/>
    public Tuner As<TRedirect>(object token = null, AddCreateBuildAction addDefaultCreateAction = AddCreateBuildAction.Yes) where TRedirect : T
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(typeof(TRedirect), token));

      var childMatcher = UnitSequenceMatcher;
      if (addDefaultCreateAction == AddCreateBuildAction.Yes)
      {
        childMatcher = new StrictUnitSequenceMatcher(Match.Type<TRedirect>(token));

        UnitSequenceMatcher
          .AddOrGetUnitSequenceMatcher(childMatcher)
          .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      }

      return new Tuner(childMatcher);
    }

    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object of type <typeparamref name="TRedirect"/>.
    /// Tune plan of building it by subsequence calls.  
    /// </summary>
    public CreationTuner<TRedirect> AsCreated<TRedirect>(object token = null) where TRedirect : T
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(typeof(TRedirect), token));
      return new CreationTuner<TRedirect>(UnitSequenceMatcher, token);
    }
    
    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object created by specified factory method.
    /// </summary>
    public void AsCreatedBy([NotNull] Func<T> factoryMethod) =>
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(_ => factoryMethod()));

    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1>([NotNull] Func<T1, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1, T2>([NotNull] Func<T1, T2, T> factoryMethod) =>
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1, T2, T3>([NotNull] Func<T1, T2, T3, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));
    
    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1, T2, T3, T4>([NotNull] Func<T1, T2, T3, T4, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod)));
    
    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1, T2, T3, T4, T5>([NotNull] Func<T1, T2, T3, T4, T5, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod)));
    
    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1, T2, T3, T4, T5, T6>([NotNull] Func<T1, T2, T3, T4, T5, T6, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod)));
    
    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public Tuner AsCreatedBy<T1, T2, T3, T4, T5, T6, T7>([NotNull] Func<T1, T2, T3, T4, T5, T6, T7, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod)));
    
    /// <inheritdoc cref="AsCreatedBy(Func{T})"/>
    public void AsCreatedBy([NotNull] Func<IBuildSession, T> factoryMethod) =>
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(factoryMethod));
 }
}