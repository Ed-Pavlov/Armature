using System;
using Armature.Sdk;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker of a tuner which tunes rules of building arguments. It's needed to ensure that not suitable tuner
/// like <see cref="IInjectPointTuner"/> are not passed to <see cref="IDependencyTuner{T}.UsingArguments"/>
/// </summary>
public interface IArgumentTuner : ITuner { }

public class ArgumentTuner : Tuner<ArgumentTuner>, IArgumentTuner
{
  public ArgumentTuner(Action<ITunerInternal> tuner) : base(tuner) { }
}