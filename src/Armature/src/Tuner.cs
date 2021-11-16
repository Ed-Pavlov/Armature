using System;
using Armature.Core;

namespace Armature;

/// <inheritdoc />
public abstract class Tuner : ITuner
{
  private readonly Action<IBuildChainPattern> _tune;
  protected Tuner(Action<IBuildChainPattern> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

  public void Tune(IBuildChainPattern buildChainPattern) => _tune(buildChainPattern);
}