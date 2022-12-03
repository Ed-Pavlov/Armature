using Armature.Core;

namespace Armature.Sdk;

/// <summary>
/// Provides possibility to extent the framework with custom Tuners by providing access to internal stuff.
/// Other "tuner" interfaces do not inherit this interface in order to not expose "internals" needed in rare advanced cases.
/// Use <see cref="ArmatureUtil.GetInternals"/> and other extension methods from Armature.Sdk namespace to access it.
/// </summary>
public interface ITuner
{
  /// <summary>
  /// The "parent" tuner or null if this is a root of a tuning sequence
  /// </summary>
  ITuner?            Parent   { get; }

  /// <summary>
  /// The root of the tree containing patterns to match a build chain
  /// </summary>
  IBuildChainPattern TreeRoot { get; }

  /// <summary>
  /// Adds a build chain pattern produced by this tuner as a child of <paramref name="node"/> or gets already added one.
  /// </summary>
  /// <returns>Returns the actual build chain pattern node, newly added or obtained from the <paramref name="node"/> </returns>
  IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node);

  int                Weight { get; }
}
