using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Adds build actions to build arguments for injection points defined by <see cref="ForParameter"/> and <see cref="ForProperty"/> tuners.
/// </summary>
public abstract class ArgumentTunerBase<T> : IInternal<TuneArgumentRecipient, int>
{
  protected readonly TuneArgumentRecipient TuneArgumentRecipientsTo;

  [PublicAPI]
  protected int Weight;

  [DebuggerStepThrough]
  protected ArgumentTunerBase(TuneArgumentRecipient tuneArgumentRecipient)
    => TuneArgumentRecipientsTo = tuneArgumentRecipient ?? throw new ArgumentNullException(nameof(tuneArgumentRecipient));

  /// <summary>
  /// Use the <paramref name="value" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseValue(T value)
    => new ArgumentSideTuner(tuner => TuneArgumentRecipientsTo(tuner, Weight).UseBuildAction(new Instance<T>(value), BuildStage.Cache));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod(Func<T> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethod<T>(_ => factoryMethod()), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<TR>(Func<TR, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<TR, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<T1, T2>(Func<T1, T2, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<T1, T2, T3>(Func<T1, T2, T3, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, object?> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethodBuildAction<T1, T2, T3, T4, T5, T6, T7, object?>(factoryMethod), BuildStage.Create));

  /// <summary>
  /// Use an instance returned by <paramref name="factoryMethod" /> as an argument for the injection point.
  /// </summary>
  public IArgumentSideTuner UseFactoryMethod<TR>(Func<IBuildSession, TR> factoryMethod)
    => new ArgumentSideTuner(
      tuner => TuneArgumentRecipientsTo(tuner, Weight)
       .UseBuildAction(new CreateWithFactoryMethod<TR>(factoryMethod), BuildStage.Create));

  #region Internals
  TuneArgumentRecipient IInternal<TuneArgumentRecipient>.Member1 => TuneArgumentRecipientsTo;
  int IInternal<TuneArgumentRecipient, int>.             Member2 => Weight;
  #endregion
}