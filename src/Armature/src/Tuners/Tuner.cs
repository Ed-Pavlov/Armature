using System;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Base class calling passed <see cref="Action{T}"/> to implement <see cref="ITuner.Tune"/> method.
/// </summary>
public abstract class Tuner<TC> : ITuner, IInternal<Action<ITunerInternal>> where TC : Tuner<TC>
{
  private readonly Action<ITunerInternal> _tune;

  [PublicAPI]
  protected short Weight;

  protected Tuner(Action<ITunerInternal> tuner) => _tune = tuner ?? throw new ArgumentNullException(nameof(tuner));

  public void Tune(ITunerInternal tuner) => _tune(tuner);

  /// <summary>
  /// Amends the weight of current registration
  /// </summary>
  public TC AmendWeight(short weight)
  {
    Weight += weight;
    return (TC) this;
  }

  Action<ITunerInternal> IInternal<Action<ITunerInternal>>.Member1                                => _tune;
}