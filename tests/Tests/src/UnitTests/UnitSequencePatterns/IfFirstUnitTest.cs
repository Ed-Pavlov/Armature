using System;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.UnitSequencePatterns;

public class IfFirstUnitTest
{
  [Test]
  public void should_match_first_unit_and_send_the_rest_to_children()
  {
    const string kind = "kind";

    var expected = new UnitId("expected", "expected");

    // --arrange
    var target   = new IfFirstUnit(new UnitPattern(kind));
    var child1 = A.Fake<IPatternTreeNode>();
    var child2 = A.Fake<IPatternTreeNode>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var sequence = new[] {new UnitId(kind, null), expected}.ToArrayTail();
    target.GatherBuildActions(sequence, 0);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(An<ArrayTail<UnitId>>.That.IsEqualTo(expected.ToArrayTail(), Comparer.OfArrayTail<UnitId>()), An<int>._))
     .MustHaveHappenedOnceAndOnly();
    A.CallTo(() => child2.GatherBuildActions(An<ArrayTail<UnitId>>.That.IsEqualTo(expected.ToArrayTail(), Comparer.OfArrayTail<UnitId>()), An<int>._))
     .MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_not_call_children_if_not_match_first_unit()
  {
    const string kind = "kind";

    var expected = new UnitId("expected", "expected");

    // --arrange
    var target   = new IfFirstUnit(new UnitPattern(kind));
    var child1 = A.Fake<IPatternTreeNode>();
    var child2 = A.Fake<IPatternTreeNode>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var sequence = new[] {new UnitId("bad", null), new UnitId(kind, null), expected}.ToArrayTail();
    target.GatherBuildActions(sequence, 0);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(default, default)).WithAnyArguments().MustNotHaveHappened();
    A.CallTo(() => child2.GatherBuildActions(default, default)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_add_self_weight_to_input_weight()
  {
    const string kind = "kind";
    const int patternWeight = 39;
    const int inputWeight   = 21;

    // --arrange
    var target   = new IfFirstUnit(new UnitPattern(kind), patternWeight);
    var child1 = A.Fake<IPatternTreeNode>();
    var child2 = A.Fake<IPatternTreeNode>();
    target.AddNode(child1);
    target.AddNode(child2);

    // --act
    var sequence = new[] {new UnitId(kind, null), new UnitId(kind, null)}.ToArrayTail();
    target.GatherBuildActions(sequence, inputWeight);

    // --assert
    A.CallTo(() => child1.GatherBuildActions(An<ArrayTail<UnitId>>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
    A.CallTo(() => child2.GatherBuildActions(An<ArrayTail<UnitId>>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_check_arguments_validity()
  {
    // --arrange
    var target = () => new IfFirstUnit(null!);

    // --assert
    target.Should().ThrowExactly<ArgumentNullException>().WithParameterName("unitPattern");
  }
}