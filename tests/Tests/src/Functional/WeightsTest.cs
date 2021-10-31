using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
// ReSharper disable All

namespace Tests.Functional;

public class WeightsTest
{
  [Test]
  public void exact_type_should_take_over_subtype()
  {
    const string expected = "expected";

    // --arrange
    var target = CreateTarget();

    target.TreatInheritorsOf<Base<int>>().UsingArguments("un" + expected);
    target.Treat<Child<int>>().AsIs().UsingArguments(expected);

    // --act
    var actual = target.Build<Child<int>>();

    // --assert
    actual!.Value.Should().Be(expected);
  }

  [Test]
  public void subtype_should_take_over_open_generic()
  {
    const string expected = "expected";

    // --arrange
    var target = CreateTarget();

    target.TreatOpenGeneric(typeof(Base<>)).AsCreated(typeof(Child<>)).UsingArguments("un" + expected);
    target.TreatInheritorsOf<Base<int>>().UsingArguments(expected);
    target.Treat<Child<int>>().AsIs();

    // --act
    var actual = target.Build<Child<int>>();

    // --assert
    actual!.Value.Should().Be(expected);
  }

  private static Builder CreateTarget()
    => new(BuildStage.Cache, BuildStage.Create)
       {
         new SkipAllUnits()
         {
           new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoList()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
         }
       };

  private record Base<T>(string Value)
  {
    public string Value { get; } = Value;
  }

  private record Child<T>(string Value) : Base<T>(Value);
}