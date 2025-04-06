using System;
using BeatyBit.Armature.Core;

namespace BeatyBit.Armature.Sdk;

public static class TunerExtension
{
  /// <summary>
  /// Provides access to internal members of tuners. See inheritors of <see cref="ITunerBase"/> for details.
  /// </summary>
  public static ITuner GetTunerInternals(this ITunerBase tuner) => (ITuner) tuner;

  /// <summary>
  /// Appends a branch of <see cref="IBuildStackPattern"/> nodes from the <paramref name="tuner"/> to <paramref name="impl"/>
  /// and return the deepest of appended nodes.
  /// </summary>
  public static IBuildStackPattern ApplyTuner(this BuildStackPatternBase impl, ITunerBase tuner)
  {
    if(impl is null) throw new ArgumentNullException(nameof(impl));
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));

    var parent = tuner.GetTunerInternals();
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
  public static IBuildStackPattern Tune(this ITuner self, IBuildStackPattern rootNode)
  {
    if(self is null) throw new ArgumentNullException(nameof(self));

    var tuner = self;
    var node  = rootNode;

    do
    {
      node  = tuner.GetOrAddNodeTo(node);
      tuner = tuner.Parent;
    } while(tuner != null);

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
}
