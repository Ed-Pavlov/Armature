using System;
using Armature.Core.Sdk;
using Armature.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Base class calling passed <see cref="Action{T}"/> to implement <see cref="ISideTuner.Tune"/> method.
/// </summary>
public abstract class SideTunerBase : ISideTuner, IInternal<Action<ITuner>>
{
  private readonly Action<ITuner> _tune;

  [PublicAPI]
  protected short Weight;

  protected SideTunerBase(Action<ITuner> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(ITuner tuner)
  {
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));
    _tune(tuner);
  }

  Action<ITuner> IInternal<Action<ITuner>>.Member1                                => _tune;
}