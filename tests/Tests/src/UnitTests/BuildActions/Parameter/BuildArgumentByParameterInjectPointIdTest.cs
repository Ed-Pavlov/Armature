using System.Linq;
using Armature;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class BuildArgumentByParameterInjectPointIdTest
{
  [Test]
  public void should_use_point_id_as_unit_key()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(int));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());

    var target = new BuildArgumentByParameterInjectPointId();

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.IsType<int>().Key(parameterInfo.Name))).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void key_should_be_null_if_no_point_id()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.ParameterType == typeof(string));

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());

    var target = new BuildArgumentByParameterInjectPointId();

    // --act
    target.Process(actual);

    // --assert
    A.CallTo(() => actual.BuildUnit(Unit.IsType<string>())).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_do_nothing_if_no_attribute()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.Name == "i");

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());

    var target = new BuildArgumentByParameterInjectPointId();

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

    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo)).GetParameters().Single(_ => _.Name == "i");

    // --arrange
    var actual = A.Fake<IBuildSession>();
    A.CallTo(() => actual.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());
    A.CallTo(() => actual.BuildUnit(Unit.IsType<int>().Key(Subject.IntId))).Returns(expected.ToBuildResult());

    var target = new BuildArgumentByParameterInjectPointId();

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.Should().Be(expected);
  }

  private class Subject
  {
    public const string IntId = nameof(IntId);

    public void Foo([Inject(IntId)] int i, [Inject]string s, bool b) { }
  }
}