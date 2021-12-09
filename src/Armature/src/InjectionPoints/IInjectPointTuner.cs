using System;
using Armature.Core;

namespace Armature;

/// <inheritdoc />
/// <summary>
/// This interface is used as a marker to ensure that not suitable tuner like <see cref="IArgumentTuner"/> not passed to <see cref="FinalTuner.InjectInto"/>
/// </summary>
public interface IInjectPointTuner : ITuner { }

public class InjectPointTuner : Tuner, IInjectPointTuner
{
  public InjectPointTuner(Action<IBuildChainPattern, int> tune) : base(tune) { }
}