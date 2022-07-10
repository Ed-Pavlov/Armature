using System;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Implementation of <see cref="ISideTuner.Tune"/> based on calling passed lambda action
/// </summary>
public abstract class SideTunerBase : ISideTuner, IInternal<Action<ITuner>>
{
  private readonly Action<ITuner> _tune;

  protected SideTunerBase(Action<ITuner> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(ITuner tuner)
  {
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));
    _tune(tuner);
  }

  Action<ITuner> IInternal<Action<ITuner>>.Member1                                => _tune;
}