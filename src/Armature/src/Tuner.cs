using System;
using Armature.Core;
using Armature.Extensibility;

namespace Armature;

/// <summary>
/// Base class calling passed <see cref="Action{T}"/> to implement <see cref="ITuner.Tune"/> method.
/// </summary>
public abstract class Tuner : ITuner, IInternal<Action<IBuildChainPattern, int>>
{
  private readonly Action<IBuildChainPattern, int> _tune;
  protected Tuner(Action<IBuildChainPattern, int> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(IBuildChainPattern buildChainPattern, int weight = 0) => _tune(buildChainPattern, weight);

  Action<IBuildChainPattern, int> IInternal<Action<IBuildChainPattern, int>>.Member1 => _tune;
}
