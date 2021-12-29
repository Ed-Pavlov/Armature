using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.Tuning.Arguments;

public class MethodArgumentTunerTest
{
  [Test]
  public void should_add_build_argument_by_parameter_type_with_tag_build_action([Values(39, "tag")] object tag)
  {
    const int expectedWeight = 298;

    // --arrange
    var target = new MethodArgumentTuner((node, _) => node);
    var root   = new Root(expectedWeight);

    // --act
    target.UseTag(tag).Tune(root);

    // --assert
    var result = root.GatherBuildActions(default, out var actionBag, 0);

    result.Should().BeTrue();
    actionBag!.Should().ContainKey(BuildStage.Create)
                  .And.Subject.Values.Single().Single()
                  .With(
                       actual =>
                       {
                         actual.Weight.Should().Be(expectedWeight);
                         actual.Entity.Should().Be(new BuildArgumentByParameterType(tag));
                       });
  }

  [Test]
  public void should_add_build_argument_by_parameter_type_with_inject_point_id_as_tag_build_action()
  {
    const int expectedWeight = 298;

    // --arrange
    var target = new MethodArgumentTuner((node, _) => node);
    var root   = new Root(expectedWeight);

    // --act
    target.UseInjectPointIdAsTag().Tune(root);

    // --assert
    var result = root.GatherBuildActions(default, out var actionBag, 0);

    result.Should().BeTrue();
    actionBag.Should().ContainKey(BuildStage.Create)
                  .And.Subject.Values.Single().Single()
                  .With(
                       actual =>
                       {
                         actual.Weight.Should().Be(expectedWeight);
                         actual.Entity.Should().Be(new BuildArgumentByParameterInjectPointId());
                       });
  }

  private class Root : BuildChainPatternBase
  {
    public Root(int expectedWeight) : base(expectedWeight) { }

    public override bool GatherBuildActions(BuildChain buildChain, out WeightedBuildActionBag? actionBag, int inputWeight) => GetOwnBuildActions(inputWeight, out actionBag);
  }
}