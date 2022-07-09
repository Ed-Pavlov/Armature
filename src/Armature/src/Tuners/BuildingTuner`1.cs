using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner<T> : BuildingTuner, IBuildingTuner<T>, IFinalAndContextTuner, ICreationTuner, IInternal<IUnitPattern>
{
  private readonly IUnitPattern _unitPattern;

  public BuildingTuner(ITunerInternal parent, CreateNode createNode, IUnitPattern unitPattern, short weight = 0)
    : base(parent, createNode, weight)
    => _unitPattern = unitPattern;

  /// <summary>
  /// Use specified <paramref name="instance"/> as a unit.
  /// </summary>
  public void AsInstance(T instance) => this.BuildBranch().UseBuildAction(new Instance<T>(instance), BuildStage.Cache);

  // IFinalAndContextTuner IFinalTuner.AmendWeight(short delta) => throw new NotImplementedException();


  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build.
  /// </summary>
  public virtual ICreationTuner As(Type type, object? tag = null)
  {
    this.BuildBranch().UseBuildAction(new RedirectType(type, tag), BuildStage.Create);

    var unitPattern = new UnitPattern(type, tag);

    IBuildChainPattern CreateTargetNode()
      => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.TargetUnit);

    return new BuildingTuner<object>(this, CreateTargetNode, unitPattern);
  }

  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build.
  /// </summary>
  public ICreationTuner As<TRedirect>(object? tag = null) => As(typeof(TRedirect), tag);

  /// <summary>
  /// Set that the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public IFinalAndContextTuner AsIs()
  {
    this.BuildBranch().UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return this;
  }

  /// <summary>
  /// Set that object of the specified <paramref name="type"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public IFinalAndContextTuner AsCreated(Type type, object? tag = null) => As(type, tag).CreatedByDefault();

  /// <summary>
  /// Set that object of the specified <typeparamref name="TRedirect"/> should be build and
  /// the <see cref="Default.CreationBuildAction"/> build action should be used to build a unit.
  /// </summary>
  public IFinalAndContextTuner AsCreated<TRedirect>(object? tag = null) => AsCreated(typeof(TRedirect), tag);

  /// <summary>
  /// Use specified <paramref name="factoryMethod"/> to build a unit.
  /// </summary>
  public IFinalAndContextTuner AsCreatedWith(Func<T> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1>(Func<T1?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1, T2>(Func<T1?, T2?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3>(Func<T1?, T2?, T3?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T?> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  /// <inheritdoc cref="AsCreatedWith(System.Func{T})" />
  public IFinalAndContextTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
  {
    this.BuildBranch().UseBuildAction(new CreateWithFactoryMethod<T>(factoryMethod), BuildStage.Create);
    return this;
  }

  IBuildingTuner<T> IBuildingTuner<T>.AmendWeight(short delta) => AmendWeight(delta, this);

  IUnitPattern IInternal<IUnitPattern>.Member1 => _unitPattern;
}
