using BeatyBit.Armature.Core;
using JetBrains.Annotations;

namespace BeatyBit.Armature.Sdk;

[PublicAPI]
public class BuildStackPattern
{
  /// <summary>
  /// Creates an instance of <see cref="BuildStackPatternTree"/> and register passed <paramref name="arguments"/>.
  /// Then the tree can be passed to <see cref="Builder.BuildUnit"/> as additional, runtime registrations.
  /// </summary>
  /// <returns>Returns null if no arguments provided</returns>
  [PublicAPI]
  public static BuildStackPatternTree? TryCreateFromArguments(object[]? arguments, short weight = -10)
    => arguments is not {Length: > 0} ? null : CreateFromArguments(arguments, weight);

  /// <summary>
  /// Creates an instance of <see cref="BuildStackPatternTree"/> and register passed <paramref name="arguments"/> if any.
  /// Then the tree can be passed to <see cref="Builder.BuildUnit"/> as additional, runtime registrations.
  /// </summary>
  public static BuildStackPatternTree CreateFromArguments(object[]? arguments, short weight = -10)
  {
    var patternTree = new BuildStackPatternTree("\"Runtime Arguments\"", weight); // decrease the weight of the "runtime" arguments by default

    if(arguments is {Length: > 0})
    {
      var rootTuner = new RootTuner(patternTree);
      DependencyTuner.UsingArguments(rootTuner, arguments);
    }

    return patternTree;
  }
}
