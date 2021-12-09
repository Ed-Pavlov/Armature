using Armature.Core;
using FakeItEasy;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildChainPatterns;

public class SkipAllUnitsTest
{
  [Test]
  public void should_skip_all_units_but_the_last_one()
  {
    var expected = new UnitId("expected", "expected");

    // --arrange
    var target   = new SkipAllUnits();
    var child1 = A.Fake<IBuildChainPattern>();
    var child2 = A.Fake<IBuildChainPattern>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var chain = new[] {new UnitId(1, null), new UnitId(null, 2), new UnitId(3, null), expected}.ToArrayTail();
    target.GatherBuildActions(chain, 0);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(An<BuildChain>.That.IsEqualTo(expected.ToArrayTail(), Comparer.OfArrayTail<UnitId>()), An<int>._))
     .MustHaveHappenedOnceAndOnly();
    A.CallTo(() => child2.GatherBuildActions(An<BuildChain>.That.IsEqualTo(expected.ToArrayTail(), Comparer.OfArrayTail<UnitId>()), An<int>._))
     .MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_sum_pattern_weight_with_input_weight()
  {
    const int patternWeight = -82;
    const int inputWeight   = 29;

    // --arrange
    var target   = new SkipAllUnits(patternWeight);
    var child1 = A.Fake<IBuildChainPattern>();
    var child2 = A.Fake<IBuildChainPattern>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var chain = new[] {new UnitId(1, null), new UnitId(null, 2), new UnitId(3, null)}.ToArrayTail();
    target.GatherBuildActions(chain, inputWeight);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(An<BuildChain>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
    A.CallTo(() => child2.GatherBuildActions(An<BuildChain>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
  }
}