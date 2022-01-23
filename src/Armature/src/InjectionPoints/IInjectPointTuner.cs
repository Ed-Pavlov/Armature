using System;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker to ensure that not suitable tuner like <see cref="IArgumentTuner"/> not passed to <see cref="DependencyTuner.InjectInto"/>
/// </summary>
public interface IInjectPointTuner : ITuner { }

public class InjectPointTuner : Tuner<InjectPointTuner>, IInjectPointTuner
{
  public InjectPointTuner(Action<TuningContext, int> tune) : base(tune) { }
}