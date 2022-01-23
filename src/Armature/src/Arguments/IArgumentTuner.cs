using System;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker of a tuner which tunes rules of building arguments. It's needed to ensure that not suitable tuner
/// like <see cref="IInjectPointTuner"/> are not passed to <see cref="DependencyTuner.UsingArguments"/>
/// </summary>
public interface IArgumentTuner : ITuner { }

public class ArgumentTuner : Tuner<ArgumentTuner>, IArgumentTuner
{
  public ArgumentTuner(Action<TuningContext, int> tune) : base(tune) { }
}