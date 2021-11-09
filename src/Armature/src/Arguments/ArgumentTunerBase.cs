using System;
using System.Diagnostics;
using Armature.Core;

namespace Armature;

public abstract class ArgumentTunerBase<T> // : UnitMatcherExtensibility
{
  protected readonly Func<IPatternTreeNode, IPatternTreeNode> TuneTreeNodePattern;

  [DebuggerStepThrough]
  protected ArgumentTunerBase(Func<IPatternTreeNode, IPatternTreeNode> tuneTreeNodePattern)
    => TuneTreeNodePattern = tuneTreeNodePattern ?? throw new ArgumentNullException(nameof(tuneTreeNodePattern));

  /// <summary>
  ///   Use the <paramref name="value" /> as an argument for the parameter.
  /// </summary>
  public IArgumentTuner UseValue(T value)
    => new ArgumentTuner(node => TuneTreeNodePattern(node).UseBuildAction(new Instance<T>(value), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod(Func<object> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethod<object>(_ => factoryMethod()), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<TR>(Func<TR?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethodBuildAction<TR, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2>(Func<T1?, T2?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3>(Func<T1?, T2?, T3?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node).UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4>(Func<T1?, T2?, T3?, T4?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1?, T2?, T3?, T4?, T5?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1?, T2?, T3?, T4?, T5?, T6?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1?, T2?, T3?, T4?, T5?, T6?, T7?, object?> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object>(factoryMethod), BuildStage.Create));

  /// <summary>
  ///   For building a value for the parameter use <paramref name="factoryMethod" /> factory method
  /// </summary>
  public IArgumentTuner UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
    => new ArgumentTuner(
      node => TuneTreeNodePattern(node)
       .UseBuildAction(new CreateWithFactoryMethod<object>(factoryMethod), BuildStage.Create));
}