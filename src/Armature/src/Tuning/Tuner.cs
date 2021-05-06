using System;
using Armature.Core;

namespace Armature
{
  /// <inheritdoc />
  public class Tuner : ITuner
  {
    private readonly Action<IPatternTreeNode> _tune;
    public Tuner(Action<IPatternTreeNode> tune) => _tune = tune ?? throw new ArgumentNullException(nameof(tune));

    public void Apply(IPatternTreeNode patternTreeNode) => _tune(patternTreeNode);
  }
}
