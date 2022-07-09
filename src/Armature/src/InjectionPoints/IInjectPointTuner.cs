using System;
using Armature.Sdk;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker to ensure that not suitable tuner like <see cref="IArgumentTuner"/> not passed to <see cref="IDependencyTuner{T}.InjectInto"/>
/// </summary>
public interface IInjectPointTuner : ITuner { }

public class InjectPointTuner : Tuner<InjectPointTuner>, IInjectPointTuner
{
  public InjectPointTuner(Action<ITunerInternal> tuner) : base(tuner) { }
}