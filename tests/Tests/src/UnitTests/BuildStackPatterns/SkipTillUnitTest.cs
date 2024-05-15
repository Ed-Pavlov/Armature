using System;
using System.Linq;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildStackPatterns
{
  public class SkipTillUnitTest
  {
    [Test]
    public void should_skip_till_unit_and_send_the_rest_to_children()
    {
      const string kind = "kind";

      var expected1 = Unit.Of("expected", "expected");
      var expected2 = Unit.Of("expected1l", "expected");

      // --arrange
      var target = new SkipTillUnit(new UnitPattern(kind));
      var child1 = A.Fake<IBuildStackPattern>();
      var child2 = A.Fake<IBuildStackPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var                     stack = TestUtil.CreateBuildStack(expected2, expected1, Unit.Of(kind), Unit.Of(2), Unit.Of(1));
      WeightedBuildActionBag? actionBag;
      target.GatherBuildActions(stack, out actionBag, 0);

      // --assert
      WeightedBuildActionBag? weightedBuildActionBag;

      A.CallTo(
            () => child1.GatherBuildActions(
                An<BuildSession.Stack>.That.IsEqualTo(TestUtil.CreateBuildStack(expected2, expected1), Comparer.OfArrayTail<UnitId>()),
                out weightedBuildActionBag,
                An<long>._))
       .MustHaveHappenedOnceAndOnly();

      WeightedBuildActionBag? actionBag1;

      A.CallTo(
            () => child2.GatherBuildActions(
                An<BuildSession.Stack>.That.IsEqualTo(TestUtil.CreateBuildStack(expected2, expected1), Comparer.OfArrayTail<UnitId>()),
                out actionBag1,
                An<long>._))
       .MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_not_send_the_rest_to_children_if_no_unit_matched()
    {
      var expected1 = Unit.Of("expected", "expected");
      var expected2 = Unit.Of("expected1l", "expected");

      // --arrange
      var target = new SkipTillUnit(new UnitPattern("absent"));
      var child1 = A.Fake<IBuildStackPattern>();
      var child2 = A.Fake<IBuildStackPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var                     stack = TestUtil.CreateBuildStack(Unit.Of(1), Unit.Of(2), expected1, expected2);
      WeightedBuildActionBag? actionBag;
      target.GatherBuildActions(stack, out actionBag, 0);

      // --assert
      WeightedBuildActionBag? weightedBuildActionBag;
      A.CallTo(() => child1.GatherBuildActions(An<BuildSession.Stack>._, out weightedBuildActionBag, An<long>._)).WithAnyArguments().MustNotHaveHappened();
      WeightedBuildActionBag? actionBag1;
      A.CallTo(() => child2.GatherBuildActions(An<BuildSession.Stack>._, out actionBag1, An<long>._)).WithAnyArguments().MustNotHaveHappened();
    }

    [Test]
    public void should_add_self_weight_to_input_weight()
    {
      const string kind          = "kind";
      const int    patternWeight = 39;
      const int    inputWeight   = 21;

      // --arrange
      var target = new SkipTillUnit(new UnitPattern(kind), patternWeight);
      var child1 = A.Fake<IBuildStackPattern>();
      var child2 = A.Fake<IBuildStackPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var                     stack = TestUtil.CreateBuildStack(Unit.Of(kind), Unit.Of(kind));
      WeightedBuildActionBag? actionBag;
      target.GatherBuildActions(stack, out actionBag, inputWeight);

      // --assert
      WeightedBuildActionBag? weightedBuildActionBag;
      A.CallTo(() => child1.GatherBuildActions(An<BuildSession.Stack>._, out weightedBuildActionBag, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
      WeightedBuildActionBag? actionBag1;
      A.CallTo(() => child2.GatherBuildActions(An<BuildSession.Stack>._, out actionBag1, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_check_arguments_validity()
    {
      // --arrange
      var target = () => new SkipTillUnit(null!);

      // --assert
      target.Should().ThrowExactly<ArgumentNullException>().WithParameterName("unitPattern");
    }

    [Test]
    public void should_return_children_merged_actions()
    {
      // --arrange
      var unitPattern = new UnitPattern(typeof(int));
      var buildStep1  = new IfFirstUnit(unitPattern);
      buildStep1.UseBuildAction(Static.Of<CreateByReflection>(), BuildStage.Cache);

      var singletonAction = new Singleton();
      var buildStep2      = new SkipTillUnit(unitPattern);
      buildStep2.UseBuildAction(singletonAction, BuildStage.Cache);

      var target = new SkipTillUnit(new UnitPattern(typeof(string)));
      target.GetOrAddNode(buildStep1);
      target.GetOrAddNode(buildStep2);

      // --act
      var actual = target.GatherBuildActions(TestUtil.CreateBuildStack(TUnit.OfType<int>(), TUnit.OfType<string>()), out var actionBag, 0);

      // --assert
      actual.Should().BeTrue();
      actionBag.Should().NotBeNull();

      actionBag![BuildStage.Cache]
         .Should()
         .HaveCount(2)
         .And
         .Subject.Select(_ => _.Entity)
         .Should()
         .BeEquivalentTo(new IBuildAction[] {Static.Of<CreateByReflection>(), singletonAction});
    }
  }
}