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
  public void should_add_build_argument_by_parameter_type_with_key_build_action([Values(39, "key")] object key)
  {
    const int expectedWeight = 298;

    // --arrange
    var target = new MethodArgumentTuner((node, _) => node);
    var root   = new Root(expectedWeight);

    // --act
    target.UseKey(key).Tune(root);

    // --assert
    var buildActionBag = root.GatherBuildActions(default, 0);

    buildActionBag.Should().ContainKey(BuildStage.Create)
                  .And.Subject.Values.Single().Single()
                  .With(
                       actual =>
                       {
                         actual.Weight.Should().Be(expectedWeight);
                         actual.Entity.Should().Be(new BuildArgumentByParameterType(key));
                       });
  }

  [Test]
  public void should_add_build_argument_by_parameter_type_with_inject_point_id_as_key_build_action()
  {
    const int expectedWeight = 298;

    // --arrange
    var target = new MethodArgumentTuner((node, _) => node);
    var root   = new Root(expectedWeight);

    // --act
    target.UseInjectPointIdAsKey().Tune(root);

    // --assert
    var buildActionBag = root.GatherBuildActions(default, 0);

    buildActionBag.Should().ContainKey(BuildStage.Create)
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

    public override WeightedBuildActionBag? GatherBuildActions(BuildChain buildChain, int inputWeight) => GetOwnBuildActions(inputWeight);
  }
}