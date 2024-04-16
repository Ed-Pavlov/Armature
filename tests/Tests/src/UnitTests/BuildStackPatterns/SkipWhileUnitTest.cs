using System;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildStackPatterns;

public class SkipWhileUnitTest
{
    [Test]
    public void should_skip_till_unit_and_send_the_rest_to_children()
    {
      const string kind = "kind";

      var expected1 = Unit.Of("expected", "expected");
      var expected2 = Unit.Of("expected1l", "expected");

      // --arrange
      var target = new SkipWhileUnit(new UnitPattern(kind));
      var child1 = A.Fake<IBuildStackPattern>();
      var child2 = A.Fake<IBuildStackPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var stack = TestUtil.CreateBuildStack(expected2, expected1, Unit.Of(kind), Unit.Of(kind), Unit.Of(kind));
      target.GatherBuildActions(stack, out var actionBag, 0);

      // --assert
      A.CallTo(
            () => child1.GatherBuildActions(
                An<BuildSession.Stack>.That.IsEqualTo(TestUtil.CreateBuildStack(expected2, expected1), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<long>._))
       .MustHaveHappenedOnceAndOnly();

      A.CallTo(
            () => child2.GatherBuildActions(
                An<BuildSession.Stack>.That.IsEqualTo(TestUtil.CreateBuildStack(expected2, expected1), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<long>._))
       .MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_send_the_last_unit_to_children_if_no_unit_matched()
    {
      const string kind = "kind";

      var expected = Unit.Of("expected", "expected");

      // --arrange
      var target = new SkipWhileUnit(new UnitPattern(kind));
      var child1 = A.Fake<IBuildStackPattern>();
      var child2 = A.Fake<IBuildStackPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var stack = TestUtil.CreateBuildStack(expected, Unit.Of(kind), Unit.Of(kind), Unit.Of(kind));
      target.GatherBuildActions(stack, out var actionBag, 0);

      // --assert
      A.CallTo(
            () => child1.GatherBuildActions(
                An<BuildSession.Stack>.That.IsEqualTo(expected.ToBuildStack(), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<long>._))
       .MustHaveHappenedOnceAndOnly();

      A.CallTo(
            () => child2.GatherBuildActions(
                An<BuildSession.Stack>.That.IsEqualTo(expected.ToBuildStack(), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<long>._))
       .MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_add_self_weight_to_input_weight()
    {
      const string kind          = "kind";
      const int    patternWeight = 39;
      const int    inputWeight   = 21;

      // --arrange
      var target = new SkipWhileUnit(new UnitPattern(kind), patternWeight);
      var child1 = A.Fake<IBuildStackPattern>();
      var child2 = A.Fake<IBuildStackPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var stack = TestUtil.CreateBuildStack(Unit.Of("not" + kind), Unit.Of(kind), Unit.Of(kind));
      target.GatherBuildActions(stack, out var actionBag, inputWeight);

      // --assert
      A.CallTo(() => child1.GatherBuildActions(An<BuildSession.Stack>._, out actionBag, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
      A.CallTo(() => child2.GatherBuildActions(An<BuildSession.Stack>._, out actionBag, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_check_arguments_validity()
    {
      // --arrange
      var target = () => new SkipWhileUnit(null!);

      // --assert
      target.Should().ThrowExactly<ArgumentNullException>().WithParameterName("unitPattern");
    }
}