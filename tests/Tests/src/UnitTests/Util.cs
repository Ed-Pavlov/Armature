using System;
using System.Linq;
using Armature;
using Armature.Core;

namespace Tests.UnitTests;

public static class Util
{
  public static BuildActionBag CreateBag(BuildStage buildStage, params IBuildAction[] buildActions) => new() {{buildStage, buildActions.ToList()}};

  public class TestPatternTreeNode : PatternTreeNodeBase
  {
    public TestPatternTreeNode(int weight = 0) : base(weight) { }
    public override WeightedBuildActionBag GatherBuildActions(ArrayTail<UnitId> unitSequence, int inputWeight) => throw new NotImplementedException();
  }
}