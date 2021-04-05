using System;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class AutowiringTest
  {
    [Test]
    public void should_inject_registered_values()
    {
      const string expectedText  = "expected 09765";
      const int    expectedValue = 93979;

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expectedText);
      target.Treat<int>().AsInstance(expectedValue);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().Be(expectedText);
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_inject_runtime_values()
    {
      const string expectedText  = "expected 09765";
      const int    expectedValue = 93979;

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>(expectedText, expectedValue);

      // --assert
      actual.Text.Should().Be(expectedText);
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_get_runtime_values_if_registered_also_presented()
    {
      const string expectedText  = "expected 09765";
      const int    expectedValue = 93979;

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(expectedText + "bad");
      target.Treat<int>().AsInstance(expectedValue   + 39);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>(expectedText, expectedValue);

      // --assert
      actual.Text.Should().Be(expectedText);
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_get_one_value_from_registration_and_another_runtime()
    {
      const string expectedText  = "expected 09765";
      const int    expectedValue = 93979;

      // --arrange
      var target = CreateTarget();
      target.Treat<string>().AsInstance(expectedText);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>(expectedValue);

      // --assert
      actual.Text.Should().Be(expectedText);
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_inject_null()
    {
      const int expectedValue = 93979;

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance(null);
      target.Treat<int>().AsInstance(expectedValue);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().BeNull();
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_use_inject_point_id_as_token()
    {
      const string expectedText  = "expected 09765";
      const int    expectedValue = 93979;

      // --arrange
      var target = CreateTarget();

      target.Treat<string>(Subject.TextParameterId).AsInstance(expectedText);
      target.Treat<int>().AsInstance(expectedValue);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().Be(expectedText);
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_fail_if_there_is_no_value_wo_token_registered()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>("token")
       .AsInstance("09765");

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      Action actual = () => target.Build<Subject>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
           new AnyUnitSequenceMatcher
           {
             // inject into constructor
             new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
              .AddBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer
                 {
                   new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
                   GetLongestConstructorBuildAction
                    .Instance // constructor with largest number of parameters has less priority
                 }),
             new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
              .AddBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer {CreateParameterValueForInjectPointBuildAction.Instance, CreateParameterValueBuildAction.Instance})
           }
         };

    private interface ISubject1
    {
      string Text { get; }
    }

    private interface ISubject2
    {
      string Text { get; }
    }

    private class Subject : ISubject1, ISubject2
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
}
