using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildListArgumentForMethodParameterTest
{
  [Test]
  public void should_build_arguments([Values(null, "key")] string key)
  {
    var expected      = new List<int>{398, 68};
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "list");

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.Is(parameterInfo).ToBuildChain());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(Unit.IsType<int>().Key(key)));
    buildUnitCall.Returns(expected.Select(_ => _.ToBuildResult().WithWeight(_)).ToList());

    var target = new BuildListArgumentForMethodParameter(key);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.As<List<int>>().Should().Equal(expected);
    A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_propagate_unit_key([Values(null, "key")] string key)
  {
    var expected      = new List<int>{398, 68};
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "list");

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.Is(parameterInfo).Key(key).ToBuildChain());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(Unit.IsType<int>().Key(key)));
    buildUnitCall.Returns(expected.Select(_ => _.ToBuildResult().WithWeight(_)).ToList());

    var target = new BuildListArgumentForMethodParameter(SpecialKey.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.As<List<int>>().Should().Equal(expected);
    A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_not_build_if_unit_is_not_collection(
      [ValueSource(nameof(not_collection_types))] ParameterInfo parameterInfo,
      [Values(null, "key")]                       object?       key)
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.Is(parameterInfo).Key(key).ToBuildChain());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(Unit.IsType<int>().Key(key)));

    var target = new BuildListArgumentForMethodParameter(SpecialKey.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustNotHaveHappened();
    buildSession.BuildResult.HasValue.Should().BeFalse();
    A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
  }

  private static IEnumerable<ParameterInfo> not_collection_types()
    => typeof(Subject).GetMethod(nameof(Subject.Boo))!.GetParameters();

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public static void Foo(IEnumerable<int> list){}
    public static void Boo(MemoryStream ms, Dictionary<int, string> dc, ArraySegment<int> @as){}
  }
}