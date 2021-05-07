using System;
using Armature.Core;

namespace Armature
{
  /// <inheritdoc />
  public abstract class Tuner : ITuner
  {
    private readonly Action<IPatternTreeNode> _tune;
    protected Tuner(Action<IPatternTreeNode> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

    public void Tune(IPatternTreeNode patternTreeNode) => _tune(patternTreeNode);
  }
}
