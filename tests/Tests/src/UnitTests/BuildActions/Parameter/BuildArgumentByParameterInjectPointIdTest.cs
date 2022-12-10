using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildArgumentByParameterInjectPointIdTest
{
  [Test]
  public void should_use_point_id_as_unit_tag()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.Of(parameterInfo).ToBuildStack());

    var target = new BuildArgumentByParameterTypeAndTag();

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(TUnit.OfType<int>(Subject.IntId))).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void tag_should_be_null_if_point_without_id()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.ParameterType == typeof(string));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.Of(parameterInfo).ToBuildStack());

    var target = new BuildArgumentByParameterTypeAndTag();

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(TUnit.OfType<string>())).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_do_nothing_if_no_attribute()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.Name == "b");

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.Of(parameterInfo).ToBuildStack());

    var target = new BuildArgumentByParameterTypeAndTag();

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
    actual.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_return_result_of_build_unit()
  {
    const string expected = "expected";

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.Name == "i");

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.Stack).Returns(Unit.Of(parameterInfo).ToBuildStack());
    A.CallTo(() => actual.BuildUnit(TUnit.OfType<int>(Subject.IntId))).Returns(expected.ToBuildResult());

    var target = new BuildArgumentByParameterTypeAndTag();

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.Should().Be(expected);
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public const string IntId = nameof(IntId);

    public static void Foo([Inject(IntId)] int i, [Inject]string s, bool b) { }
  }
}