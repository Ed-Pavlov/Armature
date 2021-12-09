using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildArgumentByParameterTypeTest
{
  [Test]
  public void should_use_parameter_type_as_unit_id([Values(null, "key")] string key)
  {
    const string expected = "expected";

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildChain).Returns(Unit.Is(parameterInfo).ToBuildChain());
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.ParameterType).Key(key))).Returns(expected.ToBuildResult());

    var target = new BuildArgumentByParameterType(key);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.ParameterType).Key(key))).MustHaveHappenedOnceAndOnly();
    actual.BuildResult.Value.Should().Be(expected);
  }

  [Test]
  public void should_use_unit_key_if_propagate_key_is_used([Values(null, "key")] string key)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildChain).Returns(Unit.Is(parameterInfo).Key(key).ToBuildChain());

    var target = new BuildArgumentByParameterType(SpecialKey.Propagate);

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.Is(parameterInfo.ParameterType).Key(key))).MustHaveHappenedOnceAndOnly();
  }

  private class Subject
  {
    public void Foo(int i) { }
  }
}