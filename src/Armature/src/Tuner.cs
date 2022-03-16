using System;
using Armature.Core;
using Armature.Extensibility;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Base class calling passed <see cref="Action{T}"/> to implement <see cref="ITuner.Tune"/> method.
/// </summary>
public abstract class Tuner<TC> : ITuner, IInternal<Action<IBuildChainPattern, int>, int> where TC: Tuner<TC>
{
  private readonly Action<IBuildChainPattern, int> _tune;

  [PublicAPI]
  protected int Weight;

  protected Tuner(Action<IBuildChainPattern, int> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(IBuildChainPattern buildChainPattern, int weight = 0) => _tune(buildChainPattern, Weight + weight);

  /// <summary>
  /// Amends the weight of current registration
  /// </summary>
  public TC AmendWeight(int weight)
  {
    Weight += weight;
    return (TC)this;
  }

  Action<IBuildChainPattern, int> IInternal<Action<IBuildChainPattern, int>>.Member1 => _tune;
  int IInternal<Action<IBuildChainPattern, int>, int>.                       Member2 => Weight;
}