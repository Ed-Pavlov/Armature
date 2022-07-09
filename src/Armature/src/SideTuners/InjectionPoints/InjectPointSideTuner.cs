using System;
using Armature.Sdk;

namespace Armature;

public class InjectPointSideTuner : SideTunerBase, IInjectPointSideTuner
{
  public InjectPointSideTuner(Action<ITunerInternal> tune) : base(tune) { }
}

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker to ensure that not suitable tuner like <see cref="IArgumentSideTuner"/> not passed to <see cref="IDependencyTuner{T}.InjectInto"/>
/// </summary>
public interface IInjectPointSideTuner : ISideTuner { }