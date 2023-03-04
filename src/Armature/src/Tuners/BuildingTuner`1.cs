using System;
using Armature.BuildActions;
using Armature.BuildActions.Caching;
using Armature.BuildActions.Creation;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;
using WeightOf = Armature.Sdk.WeightOf;

namespace Armature;

public partial class BuildingTuner<T> : SubjectTuner, IBuildingTuner<T>, ICreationTuner, IInternal<IUnitPattern, IBuildStackPattern?>
{
  [PublicAPI]
  protected readonly IUnitPattern        _unitPattern;
  [PublicAPI]
  protected          IBuildStackPattern? _buildStackPatternSubtree;

  public BuildingTuner(ITuner parent, CreateNode createNode, IUnitPattern unitPattern)
    : base(parent, createNode)
    => _unitPattern = unitPattern;

  public void AsInstance(T instance) => BuildStackPatternSubtree().UseBuildAction(new Instance<T>(instance), BuildStage.Cache);

  public virtual ICreationTuner As(Type type, object? tag = null)
  {
    if(type.IsGenericTypeDefinition)
      throw new ArgumentException($"Type should not be open generic, use {nameof(RedirectOpenGenericType)} for open generics", nameof(type));

    BuildStackPatternSubtree().UseBuildAction(new Redirect(Unit.Of(type, tag)), BuildStage.Create);

    var unitPattern = new UnitPattern(type, tag);

    IBuildStackPattern CreateTargetNode()
      => new IfFirstUnit(unitPattern, Weight + WeightOf.UnitPattern.ExactTypePattern + WeightOf.BuildStackPattern.IfFirstUnit);

    return new BuildingTuner<object>(this, CreateTargetNode, unitPattern);
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

  IBuildingTuner<T> IBuildingTuner<T>.AmendWeight(short delta) => AmendWeight(delta, this);

  protected IBuildStackPattern BuildStackPatternSubtree() => _buildStackPatternSubtree ??= this.Apply();

  #region Internals
  IUnitPattern IInternal<IUnitPattern>.                            Member1 => _unitPattern;
  IBuildStackPattern? IInternal<IUnitPattern, IBuildStackPattern?>.Member2 => _buildStackPatternSubtree;
  #endregion
}
