using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;

namespace BeatyBit.Armature;

/// <summary>
/// Tunes (overrides defaults or higher level tunings) how target unit's dependencies should be treated
/// </summary>
public interface IContextTuner : ITunerBase, IInternal<IUnitPattern, IBuildStackPattern>
{
  /// <summary>
  /// Tune up how to treat types building in the context defined by previous calls.
  /// </summary>
  ISubjectTuner BuildingIt();
}
