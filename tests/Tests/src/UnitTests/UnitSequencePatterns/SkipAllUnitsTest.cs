using Armature.Core;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.UnitSequencePatterns;

public class SkipAllUnitsTest
{
  [Test]
  public void should_skip_all_units_but_the_last_one()
  {
    var expected = new UnitId("expected", "expected");

    // --arrange
    var target   = new SkipAllUnits();
    var child1 = A.Fake<IPatternTreeNode>();
    var child2 = A.Fake<IPatternTreeNode>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var sequence = new[] {new UnitId(1, null), new UnitId(null, 2), new UnitId(3, null), expected}.ToArrayTail();
    target.GatherBuildActions(sequence, 0);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(An<ArrayTail<UnitId>>.That.IsEqualTo(expected.ToArrayTail(), Comparer.OfArrayTail<UnitId>()), An<int>._))
     .MustHaveHappenedOnceAndOnly();
    A.CallTo(() => child2.GatherBuildActions(An<ArrayTail<UnitId>>.That.IsEqualTo(expected.ToArrayTail(), Comparer.OfArrayTail<UnitId>()), An<int>._))
     .MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_sum_pattern_weight_with_input_weight()
  {
    const int patternWeight = -82;
    const int inputWeight   = 29;

    // --arrange
    var target   = new SkipAllUnits(patternWeight);
    var child1 = A.Fake<IPatternTreeNode>();
    var child2 = A.Fake<IPatternTreeNode>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var sequence = new[] {new UnitId(1, null), new UnitId(null, 2), new UnitId(3, null)}.ToArrayTail();
    target.GatherBuildActions(sequence, inputWeight);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(An<ArrayTail<UnitId>>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
    A.CallTo(() => child2.GatherBuildActions(An<ArrayTail<UnitId>>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
  }
}