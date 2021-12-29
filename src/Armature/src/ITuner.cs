using Armature.Core;

namespace Armature;

/// <summary>
/// Tunes the build chain pattern at any time, in opposite to static "tuners" like
/// <see cref="TreatingTuner"/>, <see cref="CreationTuner"/> which append build chain pattern and build actions immediately during the call of their methods.
/// </summary>
public interface ITuner
{
  /// <summary>
  /// Append pattern tree nodes to the passed <paramref name="buildChainPattern"/>
  /// </summary>
  void Tune(IBuildChainPattern buildChainPattern, int weight = 0);
}
