using System;
using Armature.Core;


namespace Armature
{
  public class TreatingTuner : TreatingTuner<object>
  {
    public TreatingTuner(IPatternTreeNode parentNode) : base(parentNode) { }
  }

  public class TreatingTuner<T> : Tuner
  {
    public TreatingTuner(IPatternTreeNode parentNode) : base(parentNode) { }


    /// <summary>
    ///   Use default creation strategy for a unit. See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
    public Tuner AsIs()
    {
      ParentNode.UseBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return this;
    }

    /// <summary>
    ///   Use specified <paramref name="instance"/> as a unit.
    /// </summary>
    public void AsInstance(T? instance) => ParentNode.UseBuildAction(BuildStage.Cache, new Singleton(instance));

    /// <summary>
    ///   Build an object of the specified <paramref name="type"/> instead.
    ///   Tune plan of creating the object by subsequence calls.
    /// </summary>
    public CreationTuner As(Type type, object? key = null)
    {
      ParentNode.UseBuildAction(BuildStage.Create, new RedirectType(type, key));
      return new CreationTuner(ParentNode, type, key);
    }

    /// <summary>
    ///   Build an object of the <typeparamref name="TRedirect"/> type instead.
    ///   Tune plan of creating the object by subsequence calls.
    /// </summary>
    public CreationTuner As<TRedirect>(object? key = null) => As(typeof(TRedirect), key);

    /// <summary>
    ///   Build an object of the specified <paramref name="type"/> instead. Also use default creation strategy for that type.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
    public Tuner AsCreated(Type type, object? key = null) => As(type, key).CreatedByDefault();

    /// <summary>
    ///   Build an object of the <typeparamref name="TRedirect"/> type instead. Also use default creation strategy for that type.
    ///   See <see cref="Default.CreationBuildAction"/> for details.
    ///   Tune plan of building it by subsequence calls.
    /// </summary>
    public Tuner AsCreated<TRedirect>(object? key = null) => AsCreated(typeof(TRedirect), key);

    /// <summary>
    ///   Use specified <paramref name="factoryMethod"/> to create a unit.
    /// </summary>
    public Tuner AsCreatedWith(Func<T> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethod<T>(_ => factoryMethod())));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1>(Func<T1?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2>(Func<T1?, T2?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3>(Func<T1?, T2?, T3?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T?> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod)));

    /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
    public Tuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
      => new(ParentNode.UseBuildAction(BuildStage.Create, new CreateWithFactoryMethod<T>(factoryMethod)));
  }
}
