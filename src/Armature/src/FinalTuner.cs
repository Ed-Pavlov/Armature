using System;
using System.Diagnostics;
using Armature.Core;

namespace Armature;

public class FinalTuner : DependencyTuner
{
  [DebuggerStepThrough]
  public FinalTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns? contextFactory)
    : base(treeRoot, tunedNode, contextFactory) { }

  /// <summary>
  /// Register Unit as an singleton with a lifetime equal to parent <see cref="BuildChainPatternTree"/>. See <see cref="Singleton" /> for details
  /// </summary>
  public void AsSingleton() => TunedNode.UseBuildAction(new Singleton(), BuildStage.Cache);
}