using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildArgumentByParameterNameTest
{
  [Test]
  public void should_use_parameter_name_as_unit_id([Values(null, "tag")] string tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.By(parameterInfo).ToBuildStack());

    var target = new BuildArgumentByParameterName(tag);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.By(parameterInfo.Name, tag), true)).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_use_unit_tag_if_propagate_tag_is_used([Values(null, "tag")] string tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.By(parameterInfo, tag).ToBuildStack());

    var target = new BuildArgumentByParameterName(ServiceTag.Propagate);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.By(parameterInfo.Name, tag), true)).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_return_result_of_build_unit([Values(null, "tag")] string tag)
  {
    const string expected = "expected";

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.By(parameterInfo).ToBuildStack());
    A.CallTo(() => actual.BuildUnit(Unit.By(parameterInfo.Name, tag), true)).Returns(expected.ToBuildResult());

    var target = new BuildArgumentByParameterName(tag);

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.Should().Be(expected);
  }


  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public static void Foo(int i) { }
  }
}