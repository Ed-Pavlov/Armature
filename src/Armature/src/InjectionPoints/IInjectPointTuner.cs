using System;
using Armature.Core;

namespace Armature;

/// <inheritdoc />
/// <summary>
///  This interface is used as a marker of a tuner which tunes rules of injection points . It's needed to ensure that not suitable tuner
/// like <see cref="IArgumentTuner"/> not passed to <see cref="FinalTuner.InjectInto"/> 
/// </summary>
public interface IInjectPointTuner : ITuner { }
  
public class InjectPointTuner : Tuner, IInjectPointTuner
{
  public InjectPointTuner(Action<IPatternTreeNode> tune) : base(tune) { }
}