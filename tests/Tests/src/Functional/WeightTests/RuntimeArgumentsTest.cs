using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.Functional.WeightTests;

public class RuntimeArgumentsTest
{
  [Test]
  public void registered_arguments_should_take_over_runtime()
  {
    const string expectedText  = "expected 09765";
    const int    expectedValue = 93979;

    // --arrange
    var target = CreateTarget();

    target.TreatAll().UsingArguments(expectedText, expectedValue);
    target.Treat<Subject>().AsIs();

    // --act
    var actual = target.Build<Subject>(expectedText + "bad", expectedValue + 38)!;

    // --assert
    actual.Text.Should().Be(expectedText);
    actual.Value.Should().Be(expectedValue);
  }

  [Test]
  public void runtime_arguments_should_take_over_registered()
  {
    const string expectedText  = "expected 09765";
    const int    expectedValue = 93979;

    // --arrange
    var target = CreateTarget();

    target.TreatAll().AmendWeight(-11).UsingArguments(expectedText + "bad", expectedValue + 32);
    target.Treat<Subject>().AsIs();

    // --act
    var actual = target.Build<Subject>(expectedText, expectedValue)!;

    // --assert
    actual.Text.Should().Be(expectedText);
    actual.Value.Should().Be(expectedValue);
  }

  private static Builder CreateTarget()
    => new(BuildStage.Cache, BuildStage.Create)
       {
           // inject into constructor
           new IfFirstUnit(new IsConstructor())
              .UseBuildAction(
                   new TryInOrder
                   {
                       new GetConstructorByInjectPointId(),       // constructor marked with [Inject] attribute has more priority
                       new GetConstructorWithMaxParametersCount() // constructor with largest number of parameters has less priority
                   },
                   BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(
                   new TryInOrder {Static.Of<BuildArgumentByParameterInjectPointId>(), Static.Of<BuildArgumentByParameterType>()},
                   BuildStage.Create)
       };

  private class Subject
  {
    public const string TextParameterId = "Text";

    public Subject([Inject(TextParameterId)] string text, int value)
    {
      Text  = text;
      Value = value;
    }

    public int    Value { get; }
    public string Text  { get; }
  }
}
