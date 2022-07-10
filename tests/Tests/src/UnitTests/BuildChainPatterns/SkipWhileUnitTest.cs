using System;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildChainPatterns;

public class SkipWhileUnitTest
{
    [Test]
    public void should_skip_till_unit_and_send_the_rest_to_children()
    {
      const string kind = "kind";

      var expected1 = new UnitId("expected", "expected");
      var expected2 = new UnitId("expected1l", "expected");

      // --arrange
      var target = new SkipWhileUnit(new UnitPattern(kind));
      var child1 = A.Fake<IBuildChainPattern>();
      var child2 = A.Fake<IBuildChainPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var chain = Util.CreateBuildChain(new UnitId(kind, null), new UnitId(kind, null), new UnitId(kind, null), expected1, expected2);
      target.GatherBuildActions(chain, out var actionBag, 0);

      // --assert
      A.CallTo(
            () => child1.GatherBuildActions(
                An<BuildChain>.That.IsEqualTo(Util.CreateBuildChain(expected1, expected2), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<int>._))
       .MustHaveHappenedOnceAndOnly();

      A.CallTo(
            () => child2.GatherBuildActions(
                An<BuildChain>.That.IsEqualTo(Util.CreateBuildChain(expected1, expected2), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<int>._))
       .MustHaveHappenedOnceAndOnly();
    }

    [Test]
    public void should_send_the_last_unit_to_children_if_no_unit_matched()
    {
      const string kind = "kind";

      var expected = new UnitId("expected", "expected");

      // --arrange
      var target = new SkipWhileUnit(new UnitPattern(kind));
      var child1 = A.Fake<IBuildChainPattern>();
      var child2 = A.Fake<IBuildChainPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var chain = Util.CreateBuildChain(new UnitId(kind, null), new UnitId(kind, null), new UnitId(kind, null), expected);
      target.GatherBuildActions(chain, out var actionBag, 0);

      // --assert
      A.CallTo(
            () => child1.GatherBuildActions(
                An<BuildChain>.That.IsEqualTo(expected.ToBuildChain(), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<int>._))
       .MustHaveHappenedOnceAndOnly();

      A.CallTo(
            () => child2.GatherBuildActions(
                An<BuildChain>.That.IsEqualTo(expected.ToBuildChain(), Comparer.OfArrayTail<UnitId>()),
                out actionBag,
                An<int>._))
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
      var child1 = A.Fake<IBuildChainPattern>();
      var child2 = A.Fake<IBuildChainPattern>();
      target.AddNode(child1);
      target.AddNode(child2);

      // --act
      var chain = Util.CreateBuildChain(new UnitId(kind, null), new UnitId(kind, null), new UnitId("not" + kind, null));
      target.GatherBuildActions(chain, out var actionBag, inputWeight);

      // --assert
      A.CallTo(() => child1.GatherBuildActions(An<BuildChain>._, out actionBag, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
      A.CallTo(() => child2.GatherBuildActions(An<BuildChain>._, out actionBag, inputWeight + patternWeight)).MustHaveHappenedOnceAndOnly();
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