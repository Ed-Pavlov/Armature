using System;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public class CreationTuner : TunerBase
{
  public CreationTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns contextFactory)
    : base(treeRoot, tunedNode, contextFactory) { }
  /// <summary>
  /// Specifies that unit should be created using default creation strategy specified in <see cref="Default.CreationBuildAction" />
  /// </summary>
  public FinalTuner CreatedByDefault() => CreateBy(Default.CreationBuildAction);

  /// <summary>
  /// Specifies that unit should be created using reflection.
  /// </summary>
  public FinalTuner CreatedByReflection() => CreateBy(Static.Of<CreateByReflection>());

  private FinalTuner CreateBy(IBuildAction buildAction)
  {
    TunedNode.UseBuildAction(buildAction, BuildStage.Create);
    return new FinalTuner(TreeRoot, TunedNode, ContextFactory!);
  }
}