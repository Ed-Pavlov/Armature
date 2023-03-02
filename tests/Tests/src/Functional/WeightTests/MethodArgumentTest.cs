using System.Diagnostics.CodeAnalysis;
using Armature;
using Armature.BuildActions.Constructor;
using Armature.BuildActions.Method;
using Armature.Core;
using Armature.UnitPatterns;
using Armature.UnitPatterns.Method;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional.WeightTests;

public class MethodArgumentTest
{
  [Test]
  public void argument_by_parameter_name_should_take_over_inject_point_id()
  {
    const string expected = "expected";

    // --arrange
    var target = CreateTarget();

    target
     .Treat<Subject>()
     .AsIs()
     .UsingArguments(
        ForParameter.Named("Value").UseValue(expected),
        ForParameter.WithInjectPoint(Subject.InjectPointId).UseValue("un" + expected));

    // --act
    var actual = target.Build<Subject>();

    // --assert
    actual!.Value.Should().Be(expected);
  }

  [Test]
  public void argument_by_parameter_inject_point_id_should_take_over_exact_type()
  {
    const string expected = "expected";

    // --arrange
    var target = CreateTarget();

    target
     .Treat<Subject>()
     .AsIs()
     .UsingArguments(
        ForParameter.OfType<string>().UseValue("un" + expected),
        ForParameter.WithInjectPoint(Subject.InjectPointId).UseValue(expected));

    // --act
    var actual = target.Build<Subject>();

    // --assert
    actual!.Value.Should().Be(expected);
  }

  [Test]
  public void argument_by_parameter_exact_type_should_take_over_type_assignability()
  {
    const string expected = "expected";

    // --arrange
    var target = CreateTarget();

    target
     .Treat<Subject>()
     .AsIs()
     .UsingArguments(ForParameter.OfType<string>().UseValue(expected), "un" + expected);

    // --act
    var actual = target.Build<Subject>();

    // --assert
    actual!.Value.Should().Be(expected);
  }

  private static Builder CreateTarget()
    => new(BuildStage.Cache, BuildStage.Create)
       {
           new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
       };

  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
  private record Subject([Inject(Subject.InjectPointId)] string Value)
  {
    public const string InjectPointId = "id";
  }
}