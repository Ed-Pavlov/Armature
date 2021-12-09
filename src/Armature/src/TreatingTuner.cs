using System;
using Armature.Core;


namespace Armature;

public class TreatingTuner : TreatingTuner<object>
{
  public TreatingTuner(IBuildChainPattern parentNode) : base(parentNode) { }
}

public class TreatingTuner<T> : FinalTuner
{
  public TreatingTuner(IBuildChainPattern parentNode) : base(parentNode) { }

  public TreatingTuner<T> AmendWeight(short weight)
  {
    Weight += weight;
    return this;
  }

  /// <summary>
  /// Use specified <paramref name="instance"/> as a unit.
  /// </summary>
  public void AsInstance(T instance) => ParentNode.UseBuildAction(new Instance<T>(instance), BuildStage.Cache);

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build.
  /// </summary>
  public CreationTuner As(Type type, object? key = null)
  {
    ParentNode.UseBuildAction(new RedirectType(type, key), BuildStage.Create);
    return new CreationTuner(ParentNode, type, key);
  }

  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build.
  /// </summary>
  public CreationTuner As<TRedirect>(object? key = null) => As(typeof(TRedirect), key);

  /// <summary>
  /// Set that the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public FinalTuner AsIs()
  {
    ParentNode.UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return this;
  }

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public FinalTuner AsCreated(Type type, object? key = null) => As(type, key).CreatedByDefault();

  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public FinalTuner AsCreated<TRedirect>(object? key = null) => AsCreated(typeof(TRedirect), key);

  /// <summary>
  /// Use specified <paramref name="factoryMethod"/> to build a unit.
  /// </summary>
  public FinalTuner AsCreatedWith(Func<T> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1>(Func<T1?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2>(Func<T1?, T2?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3>(Func<T1?, T2?, T3?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T?> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod), BuildStage.Create));

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
    => new(ParentNode.UseBuildAction(new CreateWithFactoryMethod<T>(factoryMethod), BuildStage.Create));
}