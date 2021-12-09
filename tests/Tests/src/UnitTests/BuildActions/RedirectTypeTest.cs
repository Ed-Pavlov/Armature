using System;
using System.Collections.Generic;
using System.IO;
using Armature.Core;
using Armature.Core.Sdk;
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
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<IDisposable>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<MemoryStream>().Key(key)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectType(typeof(MemoryStream), key);

    // --act
    target.Process(buildSession);

    // --assert
    buildUnitCall.MustHaveHappenedOnceAndOnly();
    buildSession.BuildResult.Value.Should().Be(expected);
  }

  [Test]
  public void should_propagate_unit_key([Values(null, "key")] string key)
  {
    var expected = new MemoryStream();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<IDisposable>().Key(key).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<MemoryStream>().Key(key)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectType(typeof(MemoryStream), SpecialKey.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_check_type_is_not_null([Values(null, "key")] object? key)
  {
    // --arrange
    var actual = () => new RedirectType(null!, key);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("redirectTo");
  }

  [Test]
  public void should_check_type_is_not_open_generic([Values(null, "key")] object? key)
  {
    // --arrange
    var actual = () => new RedirectType(typeof(List<>), key);

    // --assert
    actual.Should().ThrowExactly<ArgumentException>().WithParameterName("redirectTo").WithMessage("Type should not be open generic*");
  }
}