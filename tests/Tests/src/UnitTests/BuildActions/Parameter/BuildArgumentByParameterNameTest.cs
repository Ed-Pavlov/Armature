using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
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
    A.CallTo(() => actual.BuildChain).Returns(Unit.Is(parameterInfo).ToBuildChain());

    var target = new BuildArgumentByParameterName(tag);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.Name).Tag(tag))).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_use_unit_tag_if_propagate_tag_is_used([Values(null, "tag")] string tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildChain).Returns(Unit.Is(parameterInfo).Tag(tag).ToBuildChain());

    var target = new BuildArgumentByParameterName(SpecialTag.Propagate);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.Name).Tag(tag))).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_return_result_of_build_unit([Values(null, "tag")] string tag)
  {
    const string expected = "expected";

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildChain).Returns(Unit.Is(parameterInfo).ToBuildChain());
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.Name).Tag(tag))).Returns(expected.ToBuildResult());

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