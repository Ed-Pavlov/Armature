using System;
using Armature.Core;

namespace Armature
{
  /// <inheritdoc />
  public interface IInjectPointTuner : ITuner { }
  
  public class InjectPointTuner : Tuner, IInjectPointTuner
  {
    public InjectPointTuner(Action<IPatternTreeNode> tune) : base(tune) { }
  }
}
