using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildMethodArgumentsInDirectOrderTest
{
  [Test]
  public void should_call_build_unit_for_each_parameter_in_order()
  {
    var parametersList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters()!;

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.By(parametersList, ServiceTag.Argument).ToBuildStack());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments();

    buildUnitCall.ReturnsLazily(
        call =>
        {
          var unitId = call.Arguments.Get<UnitId>(0);
          return ((ParameterInfo) unitId.Kind!).Name.ToBuildResult();
        });

    var target = new BuildMethodArgumentsInDirectOrder();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.As<object?[]>().Should().Equal(parametersList.Select(_ => (object?) _.Name).ToArray());

    A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[0], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly()
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[1], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[2], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[3], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly());

    buildUnitCall.MustHaveHappened(parametersList.Length, Times.Exactly);
  }

  [Test]
  public void should_throw_exception_if_no_argument_build()
  {
    var parametersList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters()!;

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.By(parametersList, ServiceTag.Argument).ToBuildStack());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments();

    var target = new BuildMethodArgumentsInDirectOrder();

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should()
          .ThrowExactly<ArmatureException>().Which.Message.Should()
          .StartWith($"Argument for parameter '{parametersList[0]}'");

    buildUnitCall.MustHaveHappenedOnceExactly();
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public static void Foo(int i, int ii, string s, bool b = true) { }
  }
}