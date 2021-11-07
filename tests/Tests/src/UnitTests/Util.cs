using System;
using System.Collections.Generic;
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

  public record OtherUnitPattern : IUnitPattern
  {
    public bool Matches(UnitId unitId) => throw new NotImplementedException();
  }

  public static IEnumerable<SpecialKey> all_special_keys()
  {
    yield return SpecialKey.Any;
    yield return SpecialKey.Argument;
    yield return SpecialKey.Constructor;
    yield return SpecialKey.Propagate;
    yield return SpecialKey.PropertyList;
  }
}