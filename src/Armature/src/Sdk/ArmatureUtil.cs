using System;
using Armature.Core;

namespace Armature.Sdk;

public static class ArmatureUtil
{
  public static ITuner GetInternals(this ITunerBase tuner) => (ITuner) tuner;

  /// <summary>
  /// Appends a branch of <see cref="IBuildStackPattern"/> nodes from the <paramref name="tuner"/> to passed <paramref name="node"/>
  /// and return the deepest node to add build actions
  /// </summary>
  public static IBuildStackPattern AppendChildBuildStackPatternNodes(this IBuildStackPattern node, ITunerBase tuner)
  {
    if(node is null) throw new ArgumentNullException(nameof(node));
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));

    var parent = (ITuner)tuner;

    do
    {
      node   = parent.GetOrAddNodeTo(node);
      parent = parent.Parent;
    } while(parent != null);

    return node;
  }

  /// <summary>
  /// Adds a branch of <see cref="IBuildStackPattern"/> nodes to the Tree Root and returns the deepest node to add build actions
  /// </summary>
  public static IBuildStackPattern GetOrAddBuildStackPatternNode(this ITuner self)
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
  /// Create an instance of <see cref="BuildStackPatternTree"/> and register passed <paramref name="arguments"/>.
  /// Then the tree can be passed to <see cref="Builder.BuildUnit"/> as additional, runtime registrations.
  /// </summary>
  /// <returns>Returns null if no arguments provided</returns>
  public static BuildStackPatternTree? TryCreatePatternTreeOnArguments(object[]? arguments, short weight = -10)
    => arguments is not {Length: > 0} ? null : CreatePatternTreeOnArguments(arguments, weight);

  /// <summary>
  /// Create an instance of <see cref="BuildStackPatternTree"/> and register passed <paramref name="arguments"/> if any.
  /// Then the tree can be passed to <see cref="Builder.BuildUnit"/> as additional, runtime registrations.
  /// </summary>
  public static BuildStackPatternTree CreatePatternTreeOnArguments(object[]? arguments, short weight = -10)
  {
    var patternTree = new BuildStackPatternTree(weight); // decrease weight of the "runtime" arguments by default

    if(arguments is {Length: > 0})
    {
      var rootTuner = new RootTuner(patternTree);
      DependencyTuner.UsingArguments(rootTuner, arguments);
    }

    return patternTree;
  }
}
