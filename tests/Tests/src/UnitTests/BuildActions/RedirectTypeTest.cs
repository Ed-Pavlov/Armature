using System;
using System.Collections.Generic;
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

    var target = new RedirectType(typeof(MemoryStream), tag);

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

    var target = new RedirectType(typeof(MemoryStream), SpecialTag.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_check_type_is_not_null([Values(null, "tag")] object? tag)
  {
    // --arrange
    var actual = () => new RedirectType(null!, tag);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("redirectTo");
  }

  [Test]
  public void should_check_type_is_not_open_generic([Values(null, "tag")] object? tag)
  {
    // --arrange
    var actual = () => new RedirectType(typeof(List<>), tag);

    // --assert
    actual.Should().ThrowExactly<ArgumentException>().WithParameterName("redirectTo").WithMessage("Type should not be open generic*");
  }
}