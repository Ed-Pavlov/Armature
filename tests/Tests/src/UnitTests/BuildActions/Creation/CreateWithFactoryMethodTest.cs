using System;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.BuildActions.Creation;

public class CreateWithFactoryMethodTest
{
  [Test]
  public void should_create_unit_calling_factory_method()
  {
    const int expected = 5;

    // --arrange
    var buildSession  = new BuildSessionMock();
    var factory       = A.Fake<Func<IBuildSession, int>>();
    var callToFactory = A.CallTo(() => factory(buildSession));
    callToFactory.Returns(expected);
    var target = new CreateWithFactoryMethod<int>(factory);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    callToFactory.MustHaveHappenedOnceExactly();
  }

  [Test]
  public void should_not_do_anything_if_unit_is_already_build()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    buildSession.BuildResult = new BuildResult(283);
    Fake.ClearRecordedCalls(buildSession);

    var factory      = A.Fake<Func<IBuildSession, int>>();
    var target       = new CreateWithFactoryMethod<int>(factory);

    // --act
    target.Process(buildSession);

    // --assert
    A.CallToSet(() => buildSession.BuildResult).MustNotHaveHappened();
    A.CallTo(() => factory(null!)).WithAnyArguments().MustNotHaveHappened();
  }
}