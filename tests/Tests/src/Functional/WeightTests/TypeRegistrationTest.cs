using Armature;
using Armature.BuildActions.Constructor;
using Armature.BuildActions.Method;
using Armature.Core;
using Armature.UnitPatterns;
using Armature.UnitPatterns.Method;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional.WeightTests;

public class TypeRegistrationTest
{
  [Test]
  public void exact_type_should_take_over_inheritors()
  {
    const string expected = "expected";

    // --arrange
    var builder = CreateTarget();

    builder.TreatInheritorsOf<Base<int>>().AsIs().UsingArguments("un" + expected);
    builder.Treat<Child<int>>().AsIs().UsingArguments(expected);

    // --act
    var actual = builder.Build<Child<int>>()!;

    // --assert
    actual.Value.Should().Be(expected);
  }

  [Test]
  public void exact_type_should_take_over_open_generic()
  {
    // --arrange
    var target = CreateTarget();

    target
       .TreatOpenGeneric(typeof(Base<>))
       .AsCreated(typeof(Child<>))
       .UsingArguments("open");

    const string closed = "closed";

    target
       .Treat<Base<string>>()
       .AsCreated<Child<string>>()
       .UsingArguments(closed);

    // --act
    var actual = target.Build<Base<string>>()!;

    // --assert
    actual.Value.Should().Be(closed);
  }

  [Test]
  public void inheritors_should_take_over_open_generic()
  {
    const string expected = "expected";

    // --arrange
    var target = CreateTarget();

    target.TreatOpenGeneric(typeof(Base<>)).AsCreated(typeof(Child<>)).UsingArguments("un" + expected);
    target.TreatInheritorsOf<Base<int>>().UsingArguments(expected);
    target.Treat<Child<int>>().AsIs();

    // --act
    var actual = target.Build<Child<int>>()!;

    // --assert
    actual.Value.Should().Be(expected);
  }

  private static Builder CreateTarget()
    => new("test", BuildStage.Cache, BuildStage.Create)
       {
           new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
       };

  private record Base<T>(string Value);
  private record Child<T>(string Value) : Base<T>(Value);
}