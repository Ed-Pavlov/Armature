using System;
using Armature.Sdk;

namespace Armature;

public class ArgumentSideTuner : SideTunerBase, IArgumentSideTuner
{
  public ArgumentSideTuner(Action<ITuner> tune) : base(tune) { }
}

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker of a tuner which tunes rules of building arguments. It's needed to ensure that not suitable tuner
/// like <see cref="IInjectionPointSideTuner"/> are not passed to <see cref="IDependencyTuner{T}.UsingArguments"/>
/// </summary>
public interface IArgumentSideTuner : ISideTuner { }
