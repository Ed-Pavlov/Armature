using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildArgumentByParameterTypeTest
{
  [Test]
  public void should_use_parameter_type_as_unit_id([Values(null, "tag")] string tag)
  {
    const string expected = "expected";

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.Of(parameterInfo).ToBuildStack());
    A.CallTo(() => actual.BuildUnit(Unit.Of(parameterInfo.ParameterType, tag), true)).Returns(expected.ToBuildResult());

    var target = new BuildArgumentByParameterType(tag);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Of(parameterInfo.ParameterType, tag), true)).MustHaveHappenedOnceAndOnly();
    actual.BuildResult.Value.Should().Be(expected);
  }

  [Test]
  public void should_use_unit_tag_if_propagate_tag_is_used([Values(null, "tag")] string tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.Of(parameterInfo, tag).ToBuildStack());

    var target = new BuildArgumentByParameterType(ServiceTag.Propagate);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Of(parameterInfo.ParameterType, tag), true)).MustHaveHappenedOnceAndOnly();
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public static void Foo(int i) { }
  }
}