﻿using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

public class CreationTuner : TunerBase
{
  private readonly UnitPattern _unitPattern;

  public CreationTuner(UnitPattern unitPattern, IBuildChainPattern treeRoot, IBuildChainPattern tunedNode, AddContextPatterns contextFactory)
      : base(treeRoot, tunedNode, contextFactory)
    => _unitPattern = unitPattern;

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
    var baseWeight = Weight + WeightOf.UnitPattern.ExactTypePattern; //TODO: need to return to CreationTuner, see should_build_maybe test

    var targetUnitNode = TreeRoot.GetOrAddNode(new IfFirstUnit(_unitPattern, baseWeight + WeightOf.BuildChainPattern.TargetUnit))
                                 .TryAddContext(ContextFactory)
                                 .UseBuildAction(buildAction, BuildStage.Create);

    IBuildChainPattern AddContextTo(IBuildChainPattern node)
      => node.GetOrAddNode(new IfFirstUnit(_unitPattern, baseWeight + WeightOf.BuildChainPattern.IfFirstUnit)).TryAddContext(ContextFactory);

    return new FinalTuner(TreeRoot, targetUnitNode, AddContextTo);
  }
}