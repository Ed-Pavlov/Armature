using System;
using System.IO;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class RedirectTypeTest
{
  [Test]
  public void should_call_build_unit_with_redirected_type([Values(null, "tag")] string tag)
  {
    var expected = new MemoryStream();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<IDisposable>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<MemoryStream>().Tag(tag)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new Redirect(new UnitId(typeof(MemoryStream), tag));

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.Should().Be(expected);
  }

  [Test]
  public void should_propagate_unit_tag([Values(null, "tag")] string tag)
  {
    var expected = new MemoryStream();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<IDisposable>().Tag(tag).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<MemoryStream>().Tag(tag)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new Redirect(new UnitId(typeof(MemoryStream), SpecialTag.Propagate));

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }
}