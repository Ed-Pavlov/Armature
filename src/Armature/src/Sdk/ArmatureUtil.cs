using System;
using Armature.Core;
using JetBrains.Annotations;

namespace Armature.Sdk;

public static class ArmatureUtil
{
  /// <summary>
  /// Provides an access to internal members of tuners. See inheritors of <see cref="ITunerBase"/> for details.
  /// </summary>
  public static ITuner GetInternals(this ITunerBase tuner) => (ITuner) tuner;

  /// <summary>
  /// Appends a branch of <see cref="IBuildStackPattern"/> nodes from the <paramref name="tuner"/> to <paramref name="impl"/>
  /// and return the deepest of appended nodes.
  /// </summary>
  public static IBuildStackPattern ApplyTuner(this BuildStackPatternBase impl, ITunerBase tuner)
  {
    if(impl is null) throw new ArgumentNullException(nameof(impl));
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));

    var parent = tuner.GetInternals();
    var node   = (IBuildStackPattern) impl;

    do
    {
      node   = parent.GetOrAddNodeTo(node);
      parent = parent.Parent;
    } while(parent != null);

    return node;
  }

  /// <summary>
  /// Adds a branch of <see cref="IBuildStackPattern"/> nodes to the Tree Root and returns the deepest of added nodes.
  /// </summary>
  public static IBuildStackPattern Apply(this ITuner self)
  {
    if(self is null) throw new ArgumentNullException(nameof(self));

    var tuner = self;
    var node  = tuner.TreeRoot;

    do
    {
      node  = tuner.GetOrAddNodeTo(node);
      tuner = tuner.Parent;
    } while(tuner != null);

    return node;
  }

  /// <summary>
  /// Creates an instance of <see cref="BuildStackPatternTree"/> and register passed <paramref name="arguments"/>.
  /// Then the tree can be passed to <see cref="Builder.BuildUnit"/> as additional, runtime registrations.
  /// </summary>
  /// <returns>Returns null if no arguments provided</returns>
  [PublicAPI]
  public static BuildStackPatternTree? TryCreatePatternTreeOnArguments(object[]? arguments, short weight = -10)
    => arguments is not {Length: > 0} ? null : CreatePatternTreeOnArguments(arguments, weight);

  /// <summary>
  /// Creates an instance of <see cref="BuildStackPatternTree"/> and register passed <paramref name="arguments"/> if any.
  /// Then the tree can be passed to <see cref="Builder.BuildUnit"/> as additional, runtime registrations.
  /// </summary>
  public static BuildStackPatternTree CreatePatternTreeOnArguments(object[]? arguments, short weight = -10)
  {
    var patternTree = new BuildStackPatternTree("\"Runtime Arguments\"", weight); // decrease weight of the "runtime" arguments by default

    if(arguments is {Length: > 0})
    {
      var rootTuner = new RootTuner(patternTree);
      DependencyTuner.UsingArguments(rootTuner, arguments);
    }

    return patternTree;
  }
}
