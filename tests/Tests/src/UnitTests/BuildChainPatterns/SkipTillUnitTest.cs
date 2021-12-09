using System;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildChainPatterns
{
  public class SkipTillUnitTest
  {
    [Test]
    public void should_skip_till_unit_and_send_the_rest_to_children()
    {
      const string kind = "kind";

      var expected1 = new UnitId("expected", "expected");
      var expected2 = new UnitId("expected1l", "expected");

      // --arrange
      var target = new SkipTillUnit(new UnitPattern(kind));
      var child1 = A.Fake<IBuildChainPattern>();
      var child2 = A.Fake<IBuildChainPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var chain = new[] {new UnitId(1, null), new UnitId(2, null), new UnitId(kind, null), expected1, expected2}.ToArrayTail();
      target.GatherBuildActions(chain, 0);

      // --assert
      A.CallTo(
            () => child1.GatherBuildActions(
                An<BuildChain>.That.IsEqualTo(Util.MakeArrayTail(expected1, expected2), Comparer.OfArrayTail<UnitId>()),
                An<int>._))
       .MustHaveHappenedOnceAndOnly();

      A.CallTo(
            () => child2.GatherBuildActions(
                An<BuildChain>.That.IsEqualTo(Util.MakeArrayTail(expected1, expected2), Comparer.OfArrayTail<UnitId>()),
                An<int>._))
       .MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_not_send_the_rest_to_children_if_no_unit_matched()
    {
      var expected1 = new UnitId("expected", "expected");
      var expected2 = new UnitId("expected1l", "expected");

      // --arrange
      var target = new SkipTillUnit(new UnitPattern("absent"));
      var child1 = A.Fake<IBuildChainPattern>();
      var child2 = A.Fake<IBuildChainPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var chain = new[] {new UnitId(1, null), new UnitId(2, null), expected1, expected2}.ToArrayTail();
      target.GatherBuildActions(chain, 0);

      // --assert
      A.CallTo(() => child1.GatherBuildActions(An<BuildChain>._, An<int>._)).WithAnyArguments().MustNotHaveHappened();
      A.CallTo(() => child2.GatherBuildActions(An<BuildChain>._, An<int>._)).WithAnyArguments().MustNotHaveHappened();
    }

    [Test]
    public void should_add_self_weight_to_input_weight()
    {
      const string kind          = "kind";
      const int    patternWeight = 39;
      const int    inputWeight   = 21;

      // --arrange
      var target = new SkipTillUnit(new UnitPattern(kind), patternWeight);
      var child1 = A.Fake<IBuildChainPattern>();
      var child2 = A.Fake<IBuildChainPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var chain = new[] {new UnitId(kind, null), new UnitId(kind, null)}.ToArrayTail();
      target.GatherBuildActions(chain, inputWeight);

      // --assert
      A.CallTo(() => child1.GatherBuildActions(An<BuildChain>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
      A.CallTo(() => child2.GatherBuildActions(An<BuildChain>._, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
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
      var unitIdMatcher = Match.Type<int>(null);
      var buildStep1    = new IfFirstUnit(unitIdMatcher);
      buildStep1.UseBuildAction(Static.Of<CreateByReflection>(), BuildStage.Cache);

      var singletonAction = new Singleton();
      var buildStep2      = new SkipTillUnit(unitIdMatcher);
      buildStep2.UseBuildAction(singletonAction, BuildStage.Cache);

      var target = new SkipTillUnit(Match.Type<string>(null));
      target.GetOrAddNode(buildStep1);
      target.GetOrAddNode(buildStep2);

      // --act
      var actual = target.GatherBuildActions(new[] {Unit.IsType<string>(), Unit.IsType<int>()}.ToBuildChain(), 0);

      // --assert
      actual.Should().NotBeNull();

      // ReSharper disable once PossibleNullReferenceException
      actual[BuildStage.Cache]
         .Should()
         .HaveCount(2)
         .And
         .Subject.Select(_ => _.Entity)
         .Should()
         .BeEquivalentTo(new IBuildAction[] {Static.Of<CreateByReflection>(), singletonAction});
    }
  }
}