using System;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatingTuner : TreatingTuner<object>
  {
    public TreatingTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) { }
  }
  
  public class TreatingTuner<T> : Tuner
  {
    public TreatingTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) { }

#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of this type
    ///   created by default strategy.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public Tuner AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return new Tuner(UnitSequenceMatcher);
    }
    
    /// <summary>
    ///   For all who depends on <typeparamref name="T" /> inject <paramref name="instance" />.
    /// </summary>
    public void AsInstance([CanBeNull] T instance) => UnitSequenceMatcher.AddBuildAction(BuildStage.Cache, new SingletonBuildAction(instance));

#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of type <paramref name="type"/>.
    ///   Tune plan of creating the object by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public CreationTuner As([NotNull] Type type, object token = null)
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(type, token));
      return new CreationTuner(UnitSequenceMatcher, type, token);
    }

    /// <summary>
    ///   For all who depends on unit of type <typeparamref name="T" /> inject object of type <typeparamref name="TRedirect" />.
    ///   Tune plan of creating the object by subsequence calls.
    /// </summary>
    public CreationTuner As<TRedirect>(object token = null) => As(typeof(TRedirect), token);

#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of type
    ///   <paramref name="type"/> created by default strategy.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public Tuner AsCreated(Type type, object token = null) => As(type, token).CreatedByDefault();
    
#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of type
    ///   <typeparamref name="TRedirect" /> created by default strategy.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public Tuner AsCreated<TRedirect>(object token = null) => AsCreated(typeof(TRedirect), token);

    /// <summary>
    ///   For all who depends on <typeparamref name="T" /> inject object created by specified factory method.
    /// </summary>
    public Tuner AsCreatedWith([NotNull] Func<T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(_ => factoryMethod())));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1>([NotNull] Func<T1, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2>([NotNull] Func<T1, T2, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3>([NotNull] Func<T1, T2, T3, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4>([NotNull] Func<T1, T2, T3, T4, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5>([NotNull] Func<T1, T2, T3, T4, T5, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5, T6>([NotNull] Func<T1, T2, T3, T4, T5, T6, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>([NotNull] Func<T1, T2, T3, T4, T5, T6, T7, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith([NotNull] Func<IBuildSession, T> factoryMethod) =>
      new(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(factoryMethod)));
  }
}