using System;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public partial class BuildingTuner<T> : BuildingTuner, IBuildingTuner<T>, IFinalAndContextTuner, ICreationTuner, IInternal<IUnitPattern>
{
  private readonly IUnitPattern _unitPattern;

  public BuildingTuner(ITuner parent, CreateNode createNode, IUnitPattern unitPattern)
    : base(parent, createNode)
    => _unitPattern = unitPattern;

  public void AsInstance(T instance) => this.CreateContextBranch().UseBuildAction(new Instance<T>(instance), BuildStage.Cache);

  public virtual ICreationTuner As(Type type, object? tag = null)
  {
    this.CreateContextBranch().UseBuildAction(new RedirectType(type, tag), BuildStage.Create);

    var unitPattern = new UnitPattern(type, tag);

    IBuildChainPattern CreateTargetNode()
      => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildChainPattern.TargetUnit);

    return new BuildingTuner<object>(this, CreateTargetNode, unitPattern);
  }

  public ICreationTuner As<TRedirect>(object? tag = null) => As(typeof(TRedirect), tag);

  public IFinalAndContextTuner AsIs()
  {
    this.CreateContextBranch().UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreated(Type type, object? tag = null) => As(type, tag).CreatedByDefault();

  public IFinalAndContextTuner AsCreated<TRedirect>(object? tag = null) => AsCreated(typeof(TRedirect), tag);

  public IFinalAndContextTuner AsCreatedWith(Func<T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1>(Func<T1, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1, T2>(Func<T1, T2, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3>(Func<T1, T2, T3, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public IFinalAndContextTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
  {
    this.CreateContextBranch().UseBuildAction(new CreateWithFactoryMethod<T>(factoryMethod), BuildStage.Create);
    return this;
  }

  IBuildingTuner<T> IBuildingTuner<T>.AmendWeight(short delta) => AmendWeight(delta, this);

  IUnitPattern IInternal<IUnitPattern>.Member1 => _unitPattern;
}