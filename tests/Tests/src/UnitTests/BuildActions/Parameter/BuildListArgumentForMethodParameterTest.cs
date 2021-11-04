using System.Collections.Generic;
using System.Linq;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

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
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.Is(parameterInfo).ToBuildSequence());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(Unit.IsType<int>().Key(key)));
    buildUnitCall.Returns(expected.Select(_ => _.ToBuildResult().WithWeight(_)).ToList());

    var target = new BuildListArgumentForMethodParameter(key);

    // --act
    target.Process(buildSession);

    // --asssert
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
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.Is(parameterInfo).Key(key).ToBuildSequence());

    var buildUnitCall = A.CallTo(() => buildSession.BuildAllUnits(Unit.IsType<int>().Key(key)));
    buildUnitCall.Returns(expected.Select(_ => _.ToBuildResult().WithWeight(_)).ToList());

    var target = new BuildListArgumentForMethodParameter(SpecialKey.Propagate);

    // --act
    target.Process(buildSession);

    // --asssert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.As<List<int>>().Should().Equal(expected);
    A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
  }

  private class Subject
  {
    public static void Foo(IEnumerable<int> list){}
  }
}