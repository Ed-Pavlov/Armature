using System;
using Armature.Core;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker of a tuner which tunes rules of building arguments. It's needed to ensure that not suitable tuner
/// like <see cref="IInjectPointTuner"/> are not passed to <see cref="FinalTuner.UsingArguments"/>
/// </summary>
public interface IArgumentTuner : ITuner { }

public class ArgumentTuner : Tuner, IArgumentTuner
{
  public ArgumentTuner(Action<IBuildChainPattern, int> tune) : base(tune) { }
}
