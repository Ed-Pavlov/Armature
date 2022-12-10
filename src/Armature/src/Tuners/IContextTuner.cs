namespace Armature;

/// <summary>
/// Tunes (overrides defaults or higher level tunings) how target unit's dependencies should be treated
/// </summary>
public interface IContextTuner : ITunerBase
{
  /// <summary>
  /// Tune up how to treat types building in the context defined by previous calls.
  /// </summary>
  ISubjectTuner BuildingIt();
}
