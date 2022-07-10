using System;
using Armature.Core;

namespace Armature.Sdk;

public static class Extension
{
  /// <summary>
  /// Appends a branch of <see cref="IBuildChainPattern"/> nodes from the <paramref name="tuner"/> to passed <paramref name="node"/>
  /// and return the deepest node to add build actions
  /// </summary>
  public static IBuildChainPattern AppendContextBranch(this IBuildChainPattern node, ITuner tuner)
  {
    if(node is null) throw new ArgumentNullException(nameof(node));
    if(tuner is null) throw new ArgumentNullException(nameof(tuner));

    var parent = tuner;

    do
    {
      node   = parent.GetOrAddNodeTo(node);
      parent = parent.Parent;
    } while(parent != null);

    return node;
  }

  /// <summary>
  /// Adds a branch of <see cref="IBuildChainPattern"/> nodes to the Tree Root and returns the deepest node to add build actions
  /// </summary>
  public static IBuildChainPattern CreateContextBranch(this ITuner self)
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