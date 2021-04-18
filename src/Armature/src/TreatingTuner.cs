using System;
using Armature.Core;


namespace Armature
{
  public class TreatingTuner : TreatingTuner<object>
  {
    public TreatingTuner(IPatternTreeNode patternTreeNode) : base(patternTreeNode) { }
  }

  public class TreatingTuner<T> : Tuner
  {
    public TreatingTuner(IPatternTreeNode patternTreeNode) : base(patternTreeNode) { }

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
      PatternTreeNode.UseBuildAction(BuildStage.Create, Default.CreationBuildAction);

      return new Tuner(PatternTreeNode);
    }

    /// <summary>
    ///   For all who depends on <typeparamref name="T" /> inject <paramref name="instance" />.
    /// </summary>
    public void AsInstance(T? instance) => PatternTreeNode.UseBuildAction(BuildStage.Cache, new SingletonBuildAction(instance));

#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of type <paramref name="type"/>.
    ///   Tune plan of creating the object by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public CreationTuner As(Type type, object? key = null)
    {
      PatternTreeNode.UseBuildAction(BuildStage.Create, new RedirectTypeBuildAction(type, key));

      return new CreationTuner(PatternTreeNode, type, key);
    }

    /// <summary>
    ///   For all who depends on unit of type <typeparamref name="T" /> inject object of type <typeparamref name="TRedirect" />.
    ///   Tune plan of creating the object by subsequence calls.
    /// </summary>
    public CreationTuner As<TRedirect>(object? key = null) => As(typeof(TRedirect), key);

#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of type
    ///   <paramref name="type"/> created by default strategy.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public Tuner AsCreated(Type type, object? key = null) => As(type, key).CreatedByDefault();

#pragma warning disable 1574
    /// <summary>
    ///   For all who depends on unit of type passed into <see cref="BuildPlansCollectionExtension.Treat"/> inject object of type
    ///   <typeparamref name="TRedirect" /> created by default strategy.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
#pragma warning restore 1574
    public Tuner AsCreated<TRedirect>(object? key = null) => AsCreated(typeof(TRedirect), key);

    /// <summary>
    ///   For all who depends on <typeparamref name="T" /> inject object created by specified factory method.
    /// </summary>
    public Tuner AsCreatedWith(Func<T> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(
               BuildStage.Create,
               new CreateByFactoryMethodBuildAction<T>(_ => factoryMethod())));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1>(Func<T1?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2>(Func<T1?, T2?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3>(Func<T1?, T2?, T3?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T?> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
      => new(PatternTreeNode.UseBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(factoryMethod)));
  }
}
