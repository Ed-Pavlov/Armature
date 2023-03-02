using System;
using Armature.BuildActions.Creation;
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
}