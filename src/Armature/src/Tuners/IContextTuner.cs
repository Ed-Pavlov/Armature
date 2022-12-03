namespace Armature;

/// <summary>
/// Tunes (overrides defaults or higher level tunings) how target unit's dependencies should be treated
/// </summary>
public interface IContextTuner : ITunerBase
{
  ISubjectTuner BuildingIt();
}
