using System;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker to ensure that not suitable tuner like <see cref="IArgumentSideTuner"/>
/// not passed to <see cref="IDependencyTuner{T}.UsingInjectionPoints"/>
/// </summary>
public interface IInjectionPointSideTuner : ISideTuner { }

public class InjectionPointSideTuner : SideTuner, IInjectionPointSideTuner
{
  public InjectionPointSideTuner(Action<ITunerBase> tune) : base(tune) { }
}