using Armature.Sdk;

namespace Armature;

/// <summary>
/// Tunes the build chain pattern at any time, in opposite to static "tuners" like
/// <see cref="BuildingTuner{T}"/>, <see cref="ICreationTuner"/> which append build chain pattern and build actions immediately during the call of their methods.
/// </summary>
public interface ISideTuner
{
  /// <summary>
  /// Append pattern tree nodes to the passed <paramref name="tuner"/>
  /// </summary>
  void Tune(ITunerInternal tuner);
}

