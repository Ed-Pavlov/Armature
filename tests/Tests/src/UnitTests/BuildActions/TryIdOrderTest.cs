using System;
using System.Linq;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class TryIdOrderTest
{
  [Test]
  public void should_stop_trying_just_when_unit_is_built()
  {
    const string expected = "expected";

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    A.CallTo(() => ba2.Process(buildSession)).Invokes(_ => buildSession.BuildResult = expected.ToBuildResult());
    var target = new TryInOrder(ba1, ba2, ba3);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    A.CallTo(() => ba1.Process(buildSession)).MustHaveHappenedOnceAndOnly()
     .Then(A.CallTo(() => ba2.Process(buildSession)).MustHaveHappenedOnceAndOnly());

    A.CallTo(() => ba3.Process(default!)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_continue_trying_if_action_throws_exception()
  {
    const string expected = "expected";

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    A.CallTo(() => ba1.Process(buildSession)).Throws<ApplicationException>();
    A.CallTo(() => ba2.Process(buildSession)).Invokes(_ => buildSession.BuildResult = expected.ToBuildResult());
    var target = new TryInOrder(ba1, ba2, ba3);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
    A.CallTo(() => ba1.Process(buildSession)).MustHaveHappenedOnceAndOnly()
     .Then(A.CallTo(() => ba2.Process(buildSession)).MustHaveHappenedOnceAndOnly());

    A.CallTo(() => ba3.Process(default!)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_not_throw_exception_if_no_result_and_no_exception()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    var target = new TryInOrder(ba1, ba2, ba3);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.HasValue.Should().BeFalse();
    A.CallTo(() => ba1.Process(buildSession)).MustHaveHappenedOnceAndOnly()
     .Then(A.CallTo(() => ba2.Process(buildSession)).MustHaveHappenedOnceAndOnly())
     .Then(A.CallTo(() => ba3.Process(buildSession)).MustHaveHappenedOnceAndOnly());
  }

  [Test]
  public void should_throw_exception_if_no_result_and_exception()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    A.CallTo(() => ba1.Process(buildSession)).Throws<ApplicationException>();
    A.CallTo(() => ba3.Process(buildSession)).Throws<InvalidOperationException>();
    var target = new TryInOrder(ba1, ba2, ba3);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should()
          .ThrowExactly<AggregateException>()
          .Where(_ => _.InnerExceptions.Select(exc => exc.GetType()).SequenceEqual(new[] {typeof(ApplicationException), typeof(InvalidOperationException)}));
  }
}