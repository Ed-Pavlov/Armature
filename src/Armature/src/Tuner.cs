using System;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Base class calling passed <see cref="Action{T}"/> to implement <see cref="ITuner.Tune"/> method.
/// </summary>
public abstract class Tuner<TC> : ITuner, IInternal<Action<TuningContext, int>, int> where TC : Tuner<TC>
{
  private readonly Action<TuningContext, int> _tune;

  [PublicAPI]
  protected int Weight;

  protected Tuner(Action<TuningContext, int> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(TuningContext tuningContext, int weight = 0) => _tune(tuningContext, Weight + weight);

  /// <summary>
  /// Amends the weight of current registration
  /// </summary>
  public TC AmendWeight(int weight)
  {
    Weight += weight;
    return (TC) this;
  }

  Action<TuningContext, int> IInternal<Action<TuningContext, int>>.Member1 => _tune;
  int IInternal<Action<TuningContext, int>, int>.                  Member2 => Weight;
}