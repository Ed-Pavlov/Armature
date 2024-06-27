using System;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using JetBrains.Annotations;
using WeightOf = BeatyBit.Armature.Sdk.WeightOf;

namespace BeatyBit.Armature;

public partial class BuildingTuner<T> : SubjectTuner, IBuildingTuner<T>, ICreationTuner
{
  [PublicAPI]
  protected readonly IUnitPattern _unitPattern;

  [PublicAPI]
  protected IBuildStackPattern? _leafNode;

  public BuildingTuner(ITuner parent, CreateNode createNode, IUnitPattern unitPattern)
    : base(parent, createNode)
    => _unitPattern = unitPattern;

  public void AsInstance(T instance) => BuildStackPatternSubtree().UseBuildAction(new Instance<T>(instance), BuildStage.Create);

  public virtual ICreationTuner As(Type type, object? tag = null)
  {
    if(type.IsGenericTypeDefinition)
      throw new ArgumentException($"Type should not be open generic, use {nameof(RedirectOpenGenericType)} for open generics", nameof(type));

    BuildStackPatternSubtree().UseBuildAction(Default.CreateAsBuildAction(Unit.Of(type, tag)), BuildStage.Create);

    var unitPattern = new UnitPattern(type, tag);
    return new BuildingTuner<object>(this, CreateTargetNode, unitPattern);

    IBuildStackPattern CreateTargetNode()
      => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + Core.WeightOf.BuildStackPattern.IfFirstUnit);
  }

  public ICreationTuner As<TRedirect>(object? tag = null) => As(typeof(TRedirect), tag);

  public ISettingTuner AsIs()
  {
    BuildStackPatternSubtree().UseBuildAction(Default.CreationBuildAction, BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreated(Type type, object? tag = null) => As(type, tag).CreatedByDefault();

  public ISettingTuner AsCreated<TRedirect>(object? tag = null) => AsCreated(typeof(TRedirect), tag);

  public ISettingTuner AsCreatedWith(Func<T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1>(Func<T1, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1, T2>(Func<T1, T2, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1, T2, T3>(Func<T1, T2, T3, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1, T2, T3, T4>(Func<T1, T2, T3, T4, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, T>(factoryMethod), BuildStage.Create);
    return this;
  }

  public ISettingTuner AsCreatedWith(Func<IBuildSession, T> factoryMethod)
  {
    BuildStackPatternSubtree().UseBuildAction(new CreateWithFactoryMethod<T>(factoryMethod), BuildStage.Create);
    return this;
  }

  IBuildingTuner<T> IBuildingTuner<T>.AmendWeight(int delta) => AmendWeight(delta, this);

  protected IBuildStackPattern BuildStackPatternSubtree() => _leafNode ??= this.Tune(TreeRoot);

  #region Internals

  IUnitPattern IInternal<IUnitPattern>.                          Member1 => _unitPattern;
  IBuildStackPattern IInternal<IUnitPattern, IBuildStackPattern>.Member2 => BuildStackPatternSubtree();

  #endregion
}
