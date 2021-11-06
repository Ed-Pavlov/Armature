using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions.Creation;

public class CreateByReflectionTest
{
  [Test]
  public void should_build_no_arguments_for_default_constructor()
  {
    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType(unitType).ToBuildSequence());
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(Kind.Is(unitType).Key(SpecialKey.Constructor)));
    buildConstructor.Returns(new BuildResult(unitType.GetConstructors().Single(_ => _.GetParameters().Length == 0))); // default constructor

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildConstructor.MustHaveHappenedOnceExactly();
    A.CallTo(() => buildSession.BuildUnit(default)).WhenBuildAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_build_constructor_arguments_and_unit()
  {
    const int expected = 23948;

    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType(unitType).ToBuildSequence());
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(Kind.Is(unitType).Key(SpecialKey.Constructor)));
    buildConstructor.Returns(new BuildResult(unitType.GetConstructors().Single(_ => _.GetParameters().Length == 1))); // constructor(int i)

    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default)).WhenBuildArgumentsOfType<int>();
    buildArguments.Returns(new BuildResult(new object?[] {expected}));

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildConstructor.MustHaveHappenedOnceExactly();
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().BeOfType<Subject>().Which.Actual.Should().Be(expected);
  }

  [Test]
  public void should_not_build_value_type_with_default_ctor()
  {
    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType(unitType).ToBuildSequence());
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(Kind.Is(unitType).Key(SpecialKey.Constructor)));
    buildConstructor.Returns(new BuildResult(unitType.GetConstructors().Single(_ => _.GetParameters().Length == 0))); // default constructor

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_not_build_interface_or_abstract_type([Values(typeof(IDisposable), typeof(Stream))] Type unitType)
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType(unitType).ToBuildSequence());

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.HasValue.Should().BeFalse();
    A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
  }

  [Test]
  public void should_throw_exceptions_if_no_constructor_found()
  {
    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType(unitType).ToBuildSequence());
    A.CallTo(() => buildSession.BuildUnit(Kind.Is(unitType).Key(SpecialKey.Constructor))).Returns(default);

    var target = new CreateByReflection();

    // --act
    var actual = () => target.Process(buildSession);

    // --assert
    actual.Should()
          .ThrowExactly<ArmatureException>()
          .Which.Message.Should()
          .StartWith($"Constructor for type {unitType} is not found, check registrations for this type or default.");
  }

  [Test]
  public void should_not_do_anything_if_unit_is_already_build()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    buildSession.BuildResult = new BuildResult(283);
    Fake.ClearRecordedCalls(buildSession);

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    A.CallTo(() => buildSession.BuildUnit(default)).WithAnyArguments().MustNotHaveHappened();
    A.CallToSet(() => buildSession.BuildResult).MustNotHaveHappened();
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private struct Subject
  {
    public int Actual { get; }

    public Subject() => Actual = int.MinValue;
    public Subject(int actual) => Actual = actual;
  }
}