using System;
using Armature.Core;

namespace Armature;

/// <summary>
/// Tunes the build chain pattern at any time, in opposite to static "tuners" like
/// <see cref="TreatingTuner"/>, <see cref="CreationTuner"/> which append build chain pattern and build actions immediately during the call of their methods.
/// </summary>
public interface ITuner
{
  /// <summary>
  /// Append pattern tree nodes to the passed <paramref name="tuningContext"/>
  /// </summary>
  void Tune(TuningContext tuningContext, int weight = 0);
}

public struct TuningContext
{
  public readonly IBuildChainPattern        TreeRoot;
  public readonly IBuildChainPattern        TunedNode;
  public readonly AddContextPatterns? GetContextNode;

  public TuningContext(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns? getContextNode)
  {
    TreeRoot       = treeRoot;
    TunedNode      = tunedNode;
    GetContextNode = getContextNode;
  }
}