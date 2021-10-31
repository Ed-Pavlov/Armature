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

  [Test]
  public void argument_by_name_should_take_over_inject_point_id()
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
  public void argument_by_inject_point_id_should_take_over_exact_type()
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
  public void argument_by_exact_type_should_take_over_type_assignability()
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
         new SkipAllUnits()
         {
           new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoList()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
         }
       };

  private record Subject([Inject(Subject.InjectPointId)]string Value)
  {
    public const string InjectPointId = "id";

    public string Value { get; } = Value;
  }
  private record Base<T>(string Value) : Subject(Value);
  private record Child<T>(string Value) : Base<T>(Value);
}