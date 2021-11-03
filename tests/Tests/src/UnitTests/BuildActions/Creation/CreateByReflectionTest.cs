using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.BuildActions.Creation;

public class CreateByReflectionTest
{
  [Test]
  public void should_build_no_arguments_for_default_constructor()
  {
    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(new[] {new UnitId(unitType, null)});
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(new UnitId(unitType, SpecialKey.Constructor)));
    buildConstructor.Returns(new BuildResult(unitType.GetConstructors().Single(_ => _.GetParameters().Length == 0))); // default constructor

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildConstructor.MustHaveHappenedOnceExactly();

    A.CallTo(() => buildSession.BuildUnit(default))
     .WhenArgumentsMatch(
        arguments =>
        {
          var unitId     = arguments.Get<UnitId>(0);
          var parameters = unitId.Kind as ParameterInfo[];
          return typeof(int) == parameters?[0].ParameterType && unitId.Key == SpecialKey.Argument;
        })
     .MustNotHaveHappened();
  }

  [Test]
  public void should_build_constructor_arguments_and_unit()
  {
    const int expected = 23948;

    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(new[] {new UnitId(unitType, null)});
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(new UnitId(unitType, SpecialKey.Constructor)));

    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default))
                          .WhenArgumentsMatch(
                             arguments =>
                             {
                               var unitId     = arguments.Get<UnitId>(0);
                               var parameters = unitId.Kind as ParameterInfo[];
                               return typeof(int) == parameters?[0].ParameterType && unitId.Key == SpecialKey.Argument;
                             });

    buildConstructor.Returns(new BuildResult(unitType.GetConstructors().Single(_ => _.GetParameters().Length == 1))); // constructor(int i)
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
    A.CallTo(() => buildSession.BuildSequence).Returns(new[] {new UnitId(unitType, null)});
    var buildConstructor = A.CallTo(() => buildSession.BuildUnit(new UnitId(unitType, SpecialKey.Constructor)));
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
    A.CallTo(() => buildSession.BuildSequence).Returns(new[] {new UnitId(unitType, null)});

    var target = new CreateByReflection();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.HasValue.Should().BeFalse();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(unitType, SpecialKey.Constructor))).MustNotHaveHappened();

    A.CallTo(() => buildSession.BuildUnit(default))
     .WhenArgumentsMatch(
        arguments =>
        {
          var unitId = arguments.Get<UnitId>(0);
          return unitId.Kind is ParameterInfo[];
        })
     .MustNotHaveHappened();
  }

  [Test]
  public void should_throw_exceptions_if_no_constructor_found()
  {
    var unitType = typeof(Subject);

    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildSequence).Returns(new[] {new UnitId(unitType, null)});
    A.CallTo(() => buildSession.BuildUnit(new UnitId(unitType, SpecialKey.Constructor))).Returns(default);

    var target = new CreateByReflection();

    // --act
    Action actual = () => target.Process(buildSession);

    // --assert
    actual.Should()
          .ThrowExactly<ArmatureException>()
          .Which.Message.Should()
          .StartWith($"Constructor for type {unitType} is not found, check registrations for this type or default.");
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private struct Subject
  {
    public int Actual { get; }

    public Subject() => Actual = int.MinValue;
    public Subject(int actual) => Actual = actual;
  }
}