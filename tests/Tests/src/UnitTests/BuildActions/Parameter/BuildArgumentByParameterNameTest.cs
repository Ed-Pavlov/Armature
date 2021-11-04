using System.Linq;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class BuildArgumentByParameterNameTest
{
  [Test]
  public void should_use_parameter_name_as_unit_id([Values(null, "key")] string key)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());

    var target = new BuildArgumentByParameterName(key);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.Name).Key(key))).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_use_unit_key_if_propagate_key_is_used([Values(null, "key")] string key)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).Key(key).ToBuildSequence());

    var target = new BuildArgumentByParameterName(SpecialKey.Propagate);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.Name).Key(key))).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_return_result_of_build_unit([Values(null, "key")] string key)
  {
    const string expected = "expected";

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.Name).Key(key))).Returns(expected.ToBuildResult());

    var target = new BuildArgumentByParameterName(key);

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.Should().Be(expected);
  }


  private class Subject
  {
    public void Foo(int i) { }
  }
}