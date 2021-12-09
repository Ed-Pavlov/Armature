using System;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.Tuning.Arguments;

public class ArgumentTunerBaseTest
{
  [TestCase("string", TestName = "string")]
  [TestCase(398, TestName = "int")]
  public void should_add_instance_build_action<T>(T value)
  {
    const int expectedWeight = 38;

    // --arrange
    var target = new Impl<T>();
    var root   = new Root(expectedWeight);

    // --act
    target.UseValue(value).Tune(root);

    // --assert
    var buildActionBag = root.GatherBuildActions(default, 0);

    buildActionBag.Should().ContainKey(BuildStage.Create)
                  .And.Subject.Values.Single().Single()
                  .With(
                       actual =>
                       {
                         actual.Weight.Should().Be(expectedWeight);
                         actual.Entity.Should().Be(new Instance<T>(value));
                       });
  }

  [Test]
  public void should_add_factory_method_build_action([Values(null, "value")] string? value)
  {
    const int expectedWeight  = 38;
    var       expectedFactory = new Func<IBuildSession, string?>(_ => value);

    // --arrange
    var target  = new Impl<object?>();
    var root    = new Root(expectedWeight);

    // --act
    target.UseFactoryMethod(expectedFactory).Tune(root);

    // --assert
    var buildActionBag = root.GatherBuildActions(default, 0);

    buildActionBag.Should().ContainKey(BuildStage.Create)
                  .And.Subject.Values.Single().Single()
                  .With(
                       actual =>
                       {
                         actual.Weight.Should().Be(expectedWeight);
                         actual.Entity.Should().Be(new CreateWithFactoryMethod<string?>(expectedFactory));
                       });
  }

  private class Impl<T> : ArgumentTunerBase<T>
  {
    public Impl() : base(_ => _) { }
  }

  private class Root : BuildChainPatternBase
  {
    public Root(int expectedWeight) : base(expectedWeight) { }

    public override WeightedBuildActionBag? GatherBuildActions(BuildChain buildChain, int inputWeight) => GetOwnBuildActions(Weight);
  }
}