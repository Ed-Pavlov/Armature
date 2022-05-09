using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForParameter"/> and <see cref="ForProperty"/> tuners.
/// </summary>
public abstract class ArgumentTunerBase<T, TC> : IInternal<Func<TuningContext, int, IBuildChainPattern>, int> where TC : ArgumentTunerBase<T, TC>
{
  protected readonly Func<TuningContext, int, IBuildChainPattern> AddBuildChainPatternsTo;

  [PublicAPI]
  protected int Weight;

  [DebuggerStepThrough]
  protected ArgumentTunerBase(Func<TuningContext, int, IBuildChainPattern> addBuildChainPatterns)
    => AddBuildChainPatternsTo = addBuildChainPatterns ?? throw new ArgumentNullException(nameof(addBuildChainPatterns));

  /// <summary>
  /// Use the <paramref name="value" /> as an argument for the injection point.
  /// </summary>
  public ArgumentTuner UseValue(T? value)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight).UseBuildAction(new Instance<T>(value), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod(Func<T?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<TR>(Func<TR?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<TR, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<T1, T2>(Func<T1?, T2?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public ArgumentTuner UseFactoryMethod<TR>(Func<IBuildSession, TR?> factoryMethod)
    => new ArgumentTuner(
      (tuningContext, weight) => AddBuildChainPatternsTo(tuningContext, Weight + weight)
       .UseBuildAction(new CreateWithFactoryMethod<TR?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Amend the weight of current registration
  /// </summary>
  public TC AmendWeight(int weight)
  {
    Weight += weight;
    return (TC) this;
  }

  Func<TuningContext, int, IBuildChainPattern> IInternal<Func<TuningContext, int, IBuildChainPattern>>.Member1 => AddBuildChainPatternsTo;
  int IInternal<Func<TuningContext, int, IBuildChainPattern>, int>.                                    Member2 => Weight;
}