using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Armature.Test.Util;

namespace Armature.Test.UnitTests.BuildActions;

public class BuildMethodArgumentsInCustomOrderTest
{
  [Test]
  public void should_call_build_unit_for_each_parameter_in_direct_order()
  {
    var parametersList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters()!;

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.By(parametersList, ServiceTag.Argument).ToBuildStack());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments();

    buildUnitCall.ReturnsLazily(call =>
                                {
                                  var unitId = call.Arguments.Get<UnitId>(0);
                                  return ((ParameterInfo) unitId.Kind!).Name.ToBuildResult();
                                });

    var target = new BuildMethodArgumentsInDirectOrder();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.As<object?[]>().Should().Equal(parametersList.Select(object? (_) => _.Name).ToArray());

    A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[0], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly()
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[1], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[2], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[3], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly());

    buildUnitCall.MustHaveHappened(parametersList.Length, Times.Exactly);
  }
  
  [Test]
  public void should_call_build_unit_for_each_parameter_in_reverse_order()
  {
    var parametersList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters()!;

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.By(parametersList, ServiceTag.Argument).ToBuildStack());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments();

    buildUnitCall.ReturnsLazily(call =>
                                {
                                  var unitId = call.Arguments.Get<UnitId>(0);
                                  return ((ParameterInfo) unitId.Kind!).Name.ToBuildResult();
                                });

    var target = new BuildMethodArgumentsInReverseOrder();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.As<object?[]>().Should().Equal(parametersList.Select(object? (_) => _.Name).Reverse().ToArray());

    A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[3], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly()
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[2], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[1], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly())
     .Then(A.CallTo(() => buildSession.BuildUnit(Unit.By(parametersList[0], ServiceTag.Argument), true)).MustHaveHappenedOnceExactly());

    buildUnitCall.MustHaveHappened(parametersList.Length, Times.Exactly);
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public static void Foo(int i, int ii, string s, bool b = true) { }
  }
}
