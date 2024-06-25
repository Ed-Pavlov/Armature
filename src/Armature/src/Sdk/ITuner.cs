using BeatyBit.Armature.Core;

namespace BeatyBit.Armature.Sdk;

/// <summary>
/// Provides the possibility to extend the framework with custom Tuners by providing access to internal stuff.
/// Other "tuner" interfaces do not inherit this interface to not expose "internals" needed in rare advanced cases.
/// Use <see cref="ArmatureUtil.GetTunerInternals"/> to access it.
/// </summary>
public interface ITuner
{
  /// <summary>
  /// The "parent" tuner or null if this is a root of a tuning sequence.
  /// </summary>
  ITuner? Parent { get; }

  /// <summary>
  /// The root of the tree containing patterns to match a build stack.
  /// </summary>
  IBuildStackPattern TreeRoot { get; }

  /// <summary>
  /// Adds a build stack pattern produced by this tuner as a child of <paramref name="node"/> or gets already added one.
  /// </summary>
  /// <returns>Returns the actual build stack pattern node, newly added or obtained from the <paramref name="node"/> </returns>
  IBuildStackPattern GetOrAddNodeTo(IBuildStackPattern node);

  /// <summary>
  /// The weight used to amend the weight of build stack patterns added by the tuner.
  /// </summary>
  int Weight { get; }
}
