using System;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature;

/// <summary>
/// Implementation of <see cref="ISideTuner.ApplyTo"/> based on calling passed lambda action.
/// </summary>
public class SideTuner : ISideTuner, IInternal<Action<ITunerBase>>
{
  [PublicAPI]
  protected readonly Action<ITunerBase> _tune;

  public SideTuner(Action<ITunerBase> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void ApplyTo(ITunerBase tuner)
  {
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));
    _tune(tuner);
  }

  Action<ITunerBase> IInternal<Action<ITunerBase>>.Member1 => _tune;
}
