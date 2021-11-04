using System;
using System.Collections.Generic;
using System.IO;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class RedirectTypeTest
{
  [Test]
  public void should_call_build_unit_with_redirected_type([Values(null, "key")] string key)
  {
    var expected = new MemoryStream();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType<IDisposable>().ToBuildSequence());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<MemoryStream>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectType(typeof(MemoryStream), key);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_propagate_unit_key([Values(null, "key")] string key)
  {
    var expected = new MemoryStream();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType<IDisposable>().Key(key).ToBuildSequence());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<MemoryStream>().Key(key)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectType(typeof(MemoryStream), SpecialKey.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }
}