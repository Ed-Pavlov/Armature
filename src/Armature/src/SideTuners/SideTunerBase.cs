using System;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Base class calling passed <see cref="Action{T}"/> to implement <see cref="ISideTuner.Tune"/> method.
/// </summary>
public abstract class SideTunerBase : ISideTuner, IInternal<Action<ITunerInternal>>
{
  private readonly Action<ITunerInternal> _tune;

  [PublicAPI]
  protected short Weight;

  protected SideTunerBase(Action<ITunerInternal> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(ITunerInternal tuner) => _tune(tuner);

  Action<ITunerInternal> IInternal<Action<ITunerInternal>>.Member1                                => _tune;
}