namespace Armature
{
  /// <inheritdoc />
  /// <summary>
  ///  This interface is used as a marker of a tuner which tunes rules of resolving arguments. It's needed to check ensure that not suitable tuner
  /// like <see cref="IInjectPointTuner"/> not passed to <see cref="Tuner.UsingArguments"/> 
  /// </summary>
  public interface IArgumentTuner : ITuner { }
}
