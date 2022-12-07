using System;
using System.Collections.Generic;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

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
  public void should_continue_trying_if_action_throws_armature_exception()
  {
    const string expected = "expected";

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    A.CallTo(() => ba1.Process(buildSession)).Throws<ArmatureException>();
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
  public void should_not_continue_trying_if_action_throws_user_exception()
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
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should().ThrowExactly<ApplicationException>();
    A.CallTo(() => ba2.Process(buildSession)).MustNotHaveHappened();
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
  public void should_throw_aggregate_exception_if_no_result_and_armature_exceptions()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    A.CallTo(() => ba1.Process(buildSession)).Throws<ArmatureException>();
    A.CallTo(() => ba3.Process(buildSession)).Throws<ArmatureException>();
    var target = new TryInOrder(ba1, ba2, ba3);

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should()
          .ThrowExactly<ArmatureException>()
          .Where(_ => _.InnerExceptions.Count == 2 && _.InnerExceptions.All(inner => inner is ArmatureException));
  }

  [Test]
  public void should_call_post_process_only_for_action_returns_result()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var ba1          = A.Fake<IBuildAction>();
    var ba2          = A.Fake<IBuildAction>();
    var ba3          = A.Fake<IBuildAction>();

    A.CallTo(() => ba2.Process(buildSession)).Invokes(_ => buildSession.BuildResult = "result".ToBuildResult());
    var target = new TryInOrder(ba1, ba2, ba3);
    target.Process(buildSession);

    // --act
    target.PostProcess(buildSession);

    // --assert
    A.CallTo(() => ba2.PostProcess(buildSession)).MustHaveHappenedOnceAndOnly();

    A.CallTo(() => ba1.PostProcess(default!)).WithAnyArguments().MustNotHaveHappened();
    A.CallTo(() => ba3.PostProcess(default!)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_support_collection_style_initialization()
  {
    var expected1 = new Instance<int>(3);
    var expected2 = new CreateByReflection();

    // --act
    var target = new TryInOrder {expected1, expected2};

    // --assert
    target.Equals(new TryInOrder(expected1, expected2)).Should().BeTrue();
  }

  private static IEnumerable<IBuildAction[]> null_arguments()
  {
    yield return null!;
    yield return new IBuildAction[] {new CreateByReflection(), null!, new CreateByReflection()};
  }
  [TestCaseSource(nameof(null_arguments))]
  public void ctor_should_check_argument_for_null(IBuildAction[] buildActions)
  {
    // --arrange
    var actual = () => new TryInOrder(buildActions);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("buildActions");
  }

  [Test]
  public void add_should_check_argument_for_null()
  {
    // --arrange
    var target = new TryInOrder();

    // --act
    var actual = () => target.Add(null!);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("buildAction");
  }

  private static IEnumerable<TestCaseData> should_be_equal_source()
  {
    yield return new TestCaseData(new TryInOrder(), new TryInOrder(Empty<IBuildAction>.Array));
    const int intValue = 397;
    yield return new TestCaseData(new TryInOrder(new Instance<int>(intValue), new CreateByReflection()), new TryInOrder(new Instance<int>(intValue), new CreateByReflection()));

    var referenceEqual = new TryInOrder(new Singleton());
    yield return new TestCaseData(referenceEqual, referenceEqual);
  }

  [TestCaseSource(nameof(should_be_equal_source))]
  public void should_be_equal(TryInOrder target1, TryInOrder target2)
  {
    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  private static IEnumerable<TestCaseData> should_not_be_equal_source()
  {
    yield return new TestCaseData(new TryInOrder(), new TryInOrder(new Singleton()));

    const int intValue = 397;
    yield return new TestCaseData(
        new TryInOrder(new Instance<int>(intValue), new CreateByReflection()),
        new TryInOrder(new Instance<int>(intValue + 1), new CreateByReflection()));

    yield return new TestCaseData(
        new TryInOrder(new Singleton(), new CreateByReflection()),
        new TryInOrder(new CreateByReflection(), new Singleton()));
  }

  [TestCaseSource(nameof(should_not_be_equal_source))]
  public void should_not_be_equal(TryInOrder target1, TryInOrder target2)
  {
    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_not_be_equal_to_null() =>
      // --assert
      new TryInOrder().Equals(null).Should().BeFalse();
}