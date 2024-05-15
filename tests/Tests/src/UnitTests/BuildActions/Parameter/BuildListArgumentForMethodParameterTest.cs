using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class BuildListArgumentForMethodParameterTest
{
  [Test]
  public void should_build_arguments([Values(null, "tag")] string tag)
  {
    var expected      = new List<int>{398, 68};
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "list");

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.Of(parameterInfo).ToBuildStack());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(TUnit.OfType<int>(tag), true));
    buildUnitCall.Returns(expected.Select(_ => _.ToBuildResult().WithWeight(_)).ToList());

    var target = new BuildListArgumentForMethodParameter(tag);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.As<List<int>>().Should().Equal(expected);
    A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_propagate_unit_tag([Values(null, "tag")] string tag)
  {
    var expected      = new List<int>{398, 68};
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "list");

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.Of(parameterInfo, tag).ToBuildStack());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(TUnit.OfType<int>(tag), true));
    buildUnitCall.Returns(expected.Select(_ => _.ToBuildResult().WithWeight(_)).ToList());

    var target = new BuildListArgumentForMethodParameter(ServiceTag.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.As<List<int>>().Should().Equal(expected);
    A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_not_build_if_unit_is_not_collection(
      [ValueSource(nameof(not_collection_types))] ParameterInfo parameterInfo,
      [Values(null, "tag")]                       object?       tag)
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.Stack).Returns(Unit.Of(parameterInfo, tag).ToBuildStack());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(TUnit.OfType<int>(tag), true));

    var target = new BuildListArgumentForMethodParameter(ServiceTag.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustNotHaveHappened();
    buildSession.BuildResult.HasValue.Should().BeFalse();
    A.CallTo(() => buildSession.BuildUnit(default, true)).WithAnyArguments().MustNotHaveHappened();
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