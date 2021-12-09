using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions.Creation;

public class CreateByReflectionTest
{
  [Test]
  public void should_build_no_arguments_for_default_constructor()
  {
    var unitType = typeof(SubjectClass);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(unitType).ToBuildChain());
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(Kind.Is(unitType).Key(SpecialKey.Constructor)));
    buildConstructor.Returns(new BuildResult(unitType.GetConstructors().Single(_ => _.GetParameters().Length == 0))); // default constructor

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildConstructor.MustHaveHappenedOnceExactly();
    A.CallTo(() => buildSession.BuildUnit(default)).WhenBuildAnyArguments().MustNotHaveHappened();
    buildSession.BuildResult.Value.Should().BeOfType<SubjectClass>();
  }

  [Test]
  public void should_build_constructor_arguments_and_unit()
  {
    const int expected = 23948;

    var unitType = typeof(SubjectStruct);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(unitType).ToBuildChain());
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
    buildSession.BuildResult.Value.Should().BeOfType<SubjectStruct>().Which.Actual.Should().Be(expected);
  }

  [Test]
  public void should_not_build_value_type_with_default_ctor()
  {
    var unitType = typeof(SubjectStruct);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(unitType).ToBuildChain());
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
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(unitType).ToBuildChain());

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
    var unitType = typeof(SubjectStruct);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildChain).Returns(Unit.IsType(unitType).ToBuildChain());
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

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private struct SubjectStruct
  {
    public int Actual { get; }

    public SubjectStruct() => Actual = int.MinValue;
    public SubjectStruct(int actual) => Actual = actual;
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private class SubjectClass
  {
    public int Actual { get; }

    public SubjectClass() => Actual = int.MinValue;
    public SubjectClass(int actual) => Actual = actual;
  }
}