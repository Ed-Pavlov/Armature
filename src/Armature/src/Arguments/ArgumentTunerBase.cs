using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForParameter"/> and <see cref="ForProperty"/> tuners.
/// </summary>
public abstract class ArgumentTunerBase<T> : IInternal<Func<IBuildChainPattern, int, IBuildChainPattern>>
{
  protected readonly Func<IBuildChainPattern, int, IBuildChainPattern> AddBuildChainPatternsTo;

  [DebuggerStepThrough]
  protected ArgumentTunerBase(Func<IBuildChainPattern, int, IBuildChainPattern> addBuildChainPatterns)
    => AddBuildChainPatternsTo = addBuildChainPatterns ?? throw new ArgumentNullException(nameof(addBuildChainPatterns));

  /// <summary>
  /// Use the <paramref name="value" /> as an argument for the injection point.
  /// </summary>
  public IArgumentTuner UseValue(T? value)
    => new ArgumentTuner((node, weight) => AddBuildChainPatternsTo(node, weight).UseBuildAction(new Instance<T>(value), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod(Func<T?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight).UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<TR>(Func<TR?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<TR, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2>(Func<T1?, T2?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Build an argument for the injection point using <paramref name="factoryMethod" /> factory method.
  /// </summary>
  public IArgumentTuner UseFactoryMethod<TR>(Func<IBuildSession, TR?> factoryMethod)
    => new ArgumentTuner(
      (node, weight) => AddBuildChainPatternsTo(node, weight)
       .UseBuildAction(new CreateWithFactoryMethod<TR?>(factoryMethod), BuildStage.Create));

  Func<IBuildChainPattern, int, IBuildChainPattern> IInternal<Func<IBuildChainPattern, int, IBuildChainPattern>>.Member1 => AddBuildChainPatternsTo;
}
