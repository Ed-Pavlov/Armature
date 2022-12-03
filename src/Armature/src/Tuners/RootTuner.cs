using System;
using Armature.Core;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Tuner is used as a not null parent tuner but does not perform any tuning. <see cref="GetOrAddNodeTo"/> returns passed node w/o any changes
/// </summary>
public class RootTuner : ITuner, ITunerBase
{
  public RootTuner(IBuildChainPattern treeRoot) => TreeRoot = treeRoot ?? throw new ArgumentNullException(nameof(treeRoot));

  public ITuner?            Parent   => null;
  public IBuildChainPattern TreeRoot { get; }
  public int                Weight   => 0;

  public IBuildChainPattern GetOrAddNodeTo(IBuildChainPattern node) => node;
}
