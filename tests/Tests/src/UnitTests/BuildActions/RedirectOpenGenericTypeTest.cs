using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class RedirectOpenGenericTypeTest
{
  [Test]
  public void should_call_build_unit_with_redirected_type([Values(null, "tag")] string tag)
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<IEnumerable<int>>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>().Tag(tag)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), tag);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_propagate_unit_tag([Values(null, "tag")] string tag)
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<IEnumerable<int>>().Tag(tag).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>().Tag(tag)));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), SpecialTag.Propagate);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    buildUnitCall.MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_throw_exception_if_unit_is_not_generic()
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<string>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), null);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    var reg = new Regex("Building unit .* is not a generic type and can't be redirected.");
    actual.Should().ThrowExactly<ArmatureException>().Where(_ => reg.IsMatch(_.Message));
  }

  [Test]
  public void should_not_throw_exception_if_unit_is_not_generic_if_throw_on_error_is_not_set()
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<string>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), null, false);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should().NotThrow();
    buildUnitCall.MustNotHaveHappened();
    buildSession.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_throw_exception_if_unit_is_open_generic()
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(typeof(IEnumerable<>)).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), null);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    var reg = new Regex("Building unit .* is an open generic type and can't be redirected.");
    actual.Should().ThrowExactly<ArmatureException>().Where(_ => reg.IsMatch(_.Message));
  }

  [Test]
  public void should_not_throw_exception_if_unit_is_open_generic_if_throw_on_error_is_not_set()
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(typeof(IEnumerable<>)).ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), null, false);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should().NotThrow();
    buildUnitCall.MustNotHaveHappened();
    buildSession.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_throw_exception_generic_arguments_count_dont_match()
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<Dictionary<int, int>>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), null);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    var reg = new Regex("Generic arguments count of building unit .* and the type to redirect .* should be equal.");
    actual.Should().ThrowExactly<ArmatureException>().Where(_ => reg.IsMatch(_.Message));
  }

  [Test]
  public void should_not_throw_exception_generic_arguments_count_dont_match_if_throw_on_error_is_not_set()
  {
    var expected = new List<int>();

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType<Dictionary<int, int>>().ToBuildChain());
    var buildUnitCall = A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>()));
    buildUnitCall.Returns(expected.ToBuildResult());

    var target = new RedirectOpenGenericType(typeof(List<>), null, false);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should().NotThrow();
    buildUnitCall.MustNotHaveHappened();
    buildSession.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_check_type_argument_for_null([Values(null, "tag")] object? tag)
  {
    // --arrange
    var actual = () => new RedirectOpenGenericType(null!, tag, false);

    // --assert
    actual.Should().Throw<ArgumentNullException>().WithParameterName("redirectTo");
  }

  [Test]
  public void should_check_type_argument([Values(null, "tag")] object? tag)
  {
    // --arrange
    var actual = () => new RedirectOpenGenericType(typeof(string), tag, false);

    // --assert
    actual.Should().Throw<ArgumentException>().WithParameterName("redirectTo");
  }
}