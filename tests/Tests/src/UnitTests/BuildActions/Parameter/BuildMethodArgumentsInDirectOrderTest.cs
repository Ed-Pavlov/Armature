using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class BuildMethodArgumentsInDirectOrderTest
{
  [Test]
  public void should_call_build_unit_for_each_parameter_in_order()
  {
    var parametersList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters()!;

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.Is(parametersList).Key(SpecialKey.Argument).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments();

    buildUnitCall.ReturnsLazily(
        call =>
        {
          var unitId = call.Arguments.Get<UnitId>(0);
          return ((ParameterInfo) unitId.Kind).Name.ToBuildResult();
        });

    var target = new BuildMethodArgumentsInDirectOrder();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.As<object?[]>().Should().Equal(parametersList.Select(_ => (object?) _.Name).ToArray());

    A.CallTo(() => buildSession.BuildUnit(Unit.Is(parametersList[0]).Key(SpecialKey.Argument))).MustHaveHappenedOnceExactly()
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.Is(parametersList[1]).Key(SpecialKey.Argument))).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.Is(parametersList[2]).Key(SpecialKey.Argument))).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.Is(parametersList[3]).Key(SpecialKey.Argument))).MustHaveHappenedOnceExactly());

    buildUnitCall.MustHaveHappened(parametersList.Length, Times.Exactly);
  }

  [Test]
  public void should_throw_exception_if_no_argument_build()
  {
    var parametersList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters()!;

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.Is(parametersList).Key(SpecialKey.Argument).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments();

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