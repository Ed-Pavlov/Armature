using System;
using Armature.Core;

namespace Armature
{
  public class ArgumentStaticTuner<T> : ArgumentStaticTuner
  {
    public ArgumentStaticTuner(Func<IPatternTreeNode, IPatternTreeNode> tuneTreeNodePattern) : base(tuneTreeNodePattern) { }

    /// <summary>
    ///   Use the <paramref name="value" /> as an argument for the parameter.
    /// </summary>
    public IArgumentTuner UseValue(T? value)
      => new ArgumentTuner(node => TuneTreeNodePattern(node).UseBuildAction(new Instance<T>(value), BuildStage.Create));
  }
}
