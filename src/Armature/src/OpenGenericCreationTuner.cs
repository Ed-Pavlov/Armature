using System;
using Armature.Core;
using Armature.Core.Sdk;

namespace Armature;

public class OpenGenericCreationTuner : TunerBase
{
  public OpenGenericCreationTuner(IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns contextFactory)
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
    => new FinalTuner(TreeRoot, TunedNode .UseBuildAction(buildAction, BuildStage.Create), ContextFactory!);
}