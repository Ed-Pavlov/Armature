﻿using System;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

public class TreatingTuner : TreatingTuner<object?>
{
  public TreatingTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns contextFactory)
    : base(treeRoot, tunedNode, contextFactory) { }
}

public class TreatingTuner<T> : FinalTuner
{
  public TreatingTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns contextFactory) :
    base(treeRoot, tunedNode, contextFactory) { }

  public TreatingTuner<T> AmendWeight(short weight)
  {
    Weight += weight;
    return this;
  }

  /// <summary>
  /// Use specified <paramref name="instance"/> as a unit.
  /// </summary>
  public void AsInstance(T instance) => TunedNode.UseBuildAction(new Instance<T>(instance), BuildStage.Cache);

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build.
  /// </summary>
  public CreationTuner As(Type type, object? tag = null)
  {
    TunedNode.UseBuildAction(new RedirectType(type, tag), BuildStage.Create);

    var unitPattern = new UnitPattern(type, tag);
    var baseWeight  = Weight + WeightOf.UnitPattern.ExactTypePattern; //TODO: need to return to CreationTuner, see should_build_maybe test

    var redirectTargetNode = TreeRoot.GetOrAddNode(new IfTargetUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                     .TryAddContext(ContextFactory);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new CreationTuner(TreeRoot, redirectTargetNode, AddContextTo);
  }

  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build.
  /// </summary>
  public CreationTuner As<TRedirect>(object? tag = null) => As(typeof(TRedirect), tag);

  /// <summary>
  /// Set that the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public FinalTuner AsIs()
  {
    TunedNode.UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return this;
  }

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public FinalTuner AsCreated(Type type, object? tag = null) => As(type, tag).CreatedByDefault();

  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public FinalTuner AsCreated<TRedirect>(object? tag = null) => AsCreated(typeof(TRedirect), tag);

  /// <summary>
  /// Use specified <paramref name="factoryMethod"/> to build a unit.
  /// </summary>
  public FinalTuner AsCreatedWith(Func<T> factoryMethod)
    => new FinalTuner(TreeRoot, TunedNode.UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create), ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1>(Func<T1?, T?> factoryMethod)
    => new FinalTuner(TreeRoot, TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod), BuildStage.Create), ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2>(Func<T1?, T2?, T?> factoryMethod)
    => new FinalTuner(TreeRoot, TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod), BuildStage.Create), ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3>(Func<T1?, T2?, T3?, T?> factoryMethod)
    => new FinalTuner(
      TreeRoot,
      TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod), BuildStage.Create),
      ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, T?> factoryMethod)
    => new FinalTuner(
      TreeRoot,
      TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod), BuildStage.Create),
      ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, T?> factoryMethod)
    => new FinalTuner(
      TreeRoot,
      TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod), BuildStage.Create),
      ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T?> factoryMethod)
    => new FinalTuner(
      TreeRoot,
      TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod), BuildStage.Create),
      ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T?> factoryMethod)
    => new FinalTuner(
      TreeRoot,
      TunedNode.UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod), BuildStage.Create),
      ContextFactory!);

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public FinalTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
    => new FinalTuner(TreeRoot, TunedNode.UseBuildAction(new CreateWithFactoryMethod<T>(factoryMethod), BuildStage.Create), ContextFactory!);
}