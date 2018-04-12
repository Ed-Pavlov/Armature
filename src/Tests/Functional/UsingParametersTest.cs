using System;
using System.Collections;
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

// Resharper disable all

namespace Tests.Functional
{
  public class UsingParametersTest
  {
    [Test]
    public void should_autowire_values_passed_into_using_parameters_rather_then_registered()
    {
      const string expected = "expected 09765";

      // --arrange
      var target = CreateTarget();

      target
        .Treat<string>()
        .AsInstance("blogal");
      
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(expected);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().Be(expected);
    }
    
    [Test]
    public void should_pass_two_values()
    {
      // --arrange
      const int expectedInt = 389;
      const string expectedString = "ldksjf";

      var target = FunctionalTestHelper.CreateTarget();
      target
        .Treat<Subject>()
        .AsIs()
        .UsingInjectPointConstructor(Subject.TwoParameterCtor)
        .UsingParameters(expectedInt, expectedString);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Value.Should().Be(expectedInt);
      actual.Text.Should().Be(expectedString);
    }

    [Test]
    public void should_register_different_parameter_values_in_different_build_plans()
    {
      var target = CreateTarget();

      const string asInterfaceParameterValue = "AsInterface";
      const string asIsParameterValue = "AsIs";
      target
        .Treat<ISubject1>()
        .As<Subject>()
        .UsingParameters(asInterfaceParameterValue);

      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(asIsParameterValue);

      var asInterface = target.Build<ISubject1>();
      var asIs = target.Build<Subject>();

      // --assert
      asInterface.Text.Should().Be(asInterfaceParameterValue);
      asIs.Text.Should().Be(asIsParameterValue);
    }

    [TestCaseSource("ForParameterSource")]
    public void should_pass_null_as_parameter_value(ParameterValueTuner forParameter)
    {
      // --arrange
      var target = CreateTarget();

      target
        .Treat<string>()
        .AsInstance("938754");
      
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(forParameter.UseValue(null));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().BeNull();
    }

    [TestCaseSource("ForParameterSource")]
    public void should_use_value_for_parameter(ParameterValueTuner forParameter)
    {
      const string expected = "expected";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance("bad");
      
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(forParameter.UseValue(expected), "bad");

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().Be(expected);
    }
    
    [TestCaseSource("ForParameterSource")]
    public void should_build_value_for_parameter_using_parameter_type_and_token(ParameterValueTuner forParameter)
    {
      const string token = "token398";
      const string expected = "expected 398752";

      // --arrange
      var target = CreateTarget();
      
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(forParameter.UseToken(token), "bad");

      target
        .Treat<string>(token)
        .AsInstance(expected);

      target
        .Treat<string>("bad_token")
        .AsInstance("bad");

      target
        .Treat<string>()
        .AsInstance("bad");

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().Be(expected);
    }

    [TestCaseSource("ForParameterSource")]
    public void should_fail_if_there_is_no_value_w_token_registered(ParameterValueTuner forParameter)
    {
      // --arrange
      var target = CreateTarget();

      target
        .Treat<string>()
        .AsInstance("09765");
      
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(forParameter.UseToken("token"));
      
      // --act
      Action actual = () => target.Build<Subject>();

      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }

    [TestCaseSource("ForParameterSource")]
    public void should_use_resolver(ParameterValueTuner forParameter)
    {
      const int expectedInt = 392;

      // --arrange
      var target = CreateTarget();
      
      target.Treat<int>().AsInstance(expectedInt);
      
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(forParameter.UseResolver<int>((_, intValue) => intValue.ToString()));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Text.Should().Be(expectedInt.ToString());
    }

    [Test]
    public void should_use_personal_parameter_value_but_runtime_parameter()
    {
      const string expected = "expected29083";

      // --arrange
      var target = CreateTarget();
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(expected);

      // --act
      var actual = target.Build<Subject>("bad");

      // --assert
      actual.Text.Should().Be(expected);
    }

    [Test]
    public void should_fail_if_value_for_the_same_parameter_registered_more_than_once()
    {
      // --arrange
      var target = CreateTarget();

      var adjuster = target
        .Treat<Subject>()
        .AsIs();
      
      // --act
      Action actual = () => adjuster.UsingParameters(ForParameter.OfType<string>().UseToken("expected29083"), ForParameter.OfType<string>().UseValue("kldj"));
      
      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }

    [Test]
    public void should_not_pass_registered_parameter_when_building_dependency()
    {
      // --arrange
      var target = CreateTarget();

      target
        .Treat<Subject>()
        .AsIs();

      const string expected = "expected";
      target
        .Treat<LevelTwo>()
        .AsIs()
        .UsingParameters(expected);

      Action actual = () => target.Build<LevelTwo>();
      actual.ShouldThrow<ArmatureException>("Register string parameter only for LevelTwo class, despite that LevelOne also requires string in its .ctor registered parameter should not be propagated into LevelOne");
    }

    [Test]
    public void should_use_parameter_value_for_all_build_plans()
    {
      const string expected = "value";

      // --arrange
      var target = CreateTarget();

      target.Treat<Subject>().AsIs();
      target.Treat<LevelTwo>().AsIs();
      target.Treat<LevelThree>().AsIs();

      target
        .TreatAll()
        .UsingParameters(expected);
      
      // --act
      var actual = target.Build<LevelThree>();

      // --assert
      actual.Text.Should().Be(expected);
      actual.LevelTwo.Text.Should().Be(expected);
      actual.LevelTwo.LevelOne.Text.Should().Be(expected);
    }

    [Test]
    public void should_use_one_parameter_for_unit_and_other_for_dependencies()
    {
      const string expected = "value";
      const string expectedOnLevelThree = "levelThree";

      //--arrange
      var target = CreateTarget();

      target.Treat<Subject>().AsIs();
      target.Treat<LevelTwo>().AsIs();
      target
        .Treat<LevelThree>()
        .AsIs()
        .UsingParameters(expectedOnLevelThree);

      target
        .Building<LevelThree>()
        .TreatAll()
        .UsingParameters(expected);

      // --act
      var actual = target.Build<LevelThree>();

      // --assert
      actual.Text.Should().Be(expectedOnLevelThree, "Because {0} is registered for {1}", expectedOnLevelThree, typeof(LevelThree).Name);
      actual.LevelTwo.Text.Should().Be(expected, "Because {0} is registered for all {1} dependencies", expected, typeof(LevelThree).Name);
      actual.LevelTwo.LevelOne.Text.Should().Be(expected, "Because {0} is registered for all {1} dependencies", expected, typeof(LevelThree).Name);
    }
    
    private static IEnumerable ForParameterSource()
    {
      yield return new TestCaseData(ForParameter.OfType<string>()).SetName("OfType");
      yield return new TestCaseData(ForParameter.Named("text")).SetName("Named");
      yield return new TestCaseData(ForParameter.WithInjectPoint(null)).SetName("WithInjectPoint");
    }
    
    private static Builder CreateTarget()
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
              GetLongesConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
            }),
            

        new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
          .AddBuildAction(BuildStage.Create, new CreateParameterValueAction()) // autowiring
      };
      
      var target = new Builder(new []{BuildStage.Initialize, BuildStage.Create});
      target.Children.Add(treatAll);
      return target;
    }

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
      public const string TwoParameterCtor = "2";
      
      public int Value { get; }
      public string Text { get; }

      [Inject] // default ctor
      public Subject([Inject]string text) => Text = text;
      
      [Inject(TwoParameterCtor)]
      public Subject(string text, int value)
      {
        Text = text;
        Value = value;
      }
    }
    
    private class LevelTwo : Subject
    {
      public readonly Subject LevelOne;

      public LevelTwo(Subject levelOne, string text) : base(text) => LevelOne = levelOne;
    }

    private class LevelThree : Subject
    {
      public readonly LevelTwo LevelTwo;

      public LevelThree(LevelTwo levelTwo, string text) : base(text) => LevelTwo = levelTwo;
    }
  }
}