namespace BeatyBit.Armature;

/// <summary>
/// Tunes the build stack pattern at any time, in opposite to static "tuners" like
/// <see cref="BuildingTuner{T}"/>, <see cref="ICreationTuner"/> which append build stack pattern and build actions immediately during the call
/// of their methods.
/// </summary>
public interface ISideTuner
{
  /// <summary>
  /// Append build stack pattern tree nodes to the passed <paramref name="tuner"/>
  /// </summary>
  void ApplyTo(ITunerBase tuner);
}

