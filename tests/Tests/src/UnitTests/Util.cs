using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;

namespace Tests.UnitTests;

public static class Util
{
  public static BuildActionBag CreateBag(BuildStage buildStage, params IBuildAction[] buildActions) => new() {{buildStage, buildActions.ToList()}};

  [DebuggerStepThrough]
  public static BuildChain MakeArrayTail(params UnitId[] array) => new BuildChain(array, 0);

  [DebuggerStepThrough]
  public static BuildChain ToArrayTail(this UnitId[] array) => new(array, 0);

  [DebuggerStepThrough]
  public static BuildChain ToArrayTail(this UnitId item) => new(new []{item}, 0);

  public class TestBuildChainPattern : BuildChainPatternBase
  {
    public TestBuildChainPattern(int                   weight = 0) : base(weight) { }
    public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight) => throw new NotSupportedException();
  }

  public record OtherUnitPattern : IUnitPattern
  {
    public bool Matches(UnitId unitId) => throw new NotSupportedException();
  }

  public static IEnumerable<SpecialTag> all_special_tags()
  {
    yield return SpecialTag.Any;
    yield return SpecialTag.Argument;
    yield return SpecialTag.Constructor;
    yield return SpecialTag.Propagate;
    yield return SpecialTag.PropertyList;
  }
}