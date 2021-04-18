using System;
using System.Collections;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

namespace Tests.Functional
{
  public class InjectParameterValueTest
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
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(expected);

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().Be(expected);
    }

    [Test]
    public void should_pass_two_values()
    {
      // --arrange
      const int    expectedInt    = 389;
      const string expectedString = "ldksjf";

      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingInjectPointConstructor(LevelOne.TwoParameterCtor)
       .UsingParameters(expectedInt, expectedString);

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Value.Should().Be(expectedInt);
      actual.Text.Should().Be(expectedString);
    }

    [Test]
    public void should_register_different_parameter_values_in_different_build_plans()
    {
      var target = CreateTarget();

      const string asInterfaceParameterValue = "AsInterface";
      const string asIsParameterValue        = "AsIs";

      target
       .Treat<ISubject1>()
       .AsCreated<LevelOne>()
       .UsingParameters(asInterfaceParameterValue);

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(asIsParameterValue);

      var asInterface = target.Build<ISubject1>();
      var asIs        = target.Build<LevelOne>();

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
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(forParameter.UseValue(null));

      // --act
      var actual = target.Build<LevelOne>();

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
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(forParameter.UseValue(expected), "bad");

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().Be(expected);
    }

    [TestCaseSource("ForParameterSource")]
    public void should_build_value_for_parameter_using_parameter_type_and_key(ParameterValueTuner forParameter)
    {
      const string key      = "key398";
      const string expected = "expected 398752";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(forParameter.UseKey(key), "bad");

      target
       .Treat<string>(key)
       .AsInstance(expected);

      target
       .Treat<string>("bad_key")
       .AsInstance("bad");

      target
       .Treat<string>()
       .AsInstance("bad");

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().Be(expected);
    }

    [TestCaseSource("ForParameterSource")]
    public void should_fail_if_there_is_no_value_w_key_registered(ParameterValueTuner forParameter)
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>()
       .AsInstance("09765");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(forParameter.UseKey("key"));

      // --act
      Action actual = () => target.Build<LevelOne>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [TestCaseSource("ForParameterSource")]
    public void should_use_factory_method(ParameterValueTuner forParameter)
    {
      const int expectedInt = 392;

      // --arrange
      var target = CreateTarget();

      target.Treat<int>().AsInstance(expectedInt);

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(forParameter.UseFactoryMethod<int>(intValue => intValue.ToString()));

      // --act
      var actual = target.Build<LevelOne>();

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
       .Treat<LevelOne>()
       .AsIs()
       .UsingParameters(expected);

      // --act
      var actual = target.Build<LevelOne>("bad");

      // --assert
      actual.Text.Should().Be(expected);
    }

    [Test]
    public void should_fail_if_value_for_the_same_parameter_registered_more_than_once()
    {
      // --arrange
      var target = CreateTarget();

      var adjuster = target
                    .Treat<LevelOne>()
                    .AsIs();

      // --act
      Action actual = () => adjuster.UsingParameters(
                        ForParameter.OfType<string>().UseKey("expected29083"),
                        ForParameter.OfType<string>().UseValue("kldj"));

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [Test]
    public void should_not_pass_registered_parameter_when_building_dependency()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs();

      const string expected = "expected";

      target
       .Treat<LevelTwo>()
       .AsIs()
       .UsingParameters(expected);

      Action actual = () => target.Build<LevelTwo>();

      actual.Should()
            .Throw<ArmatureException>(
               "Register string parameter only for LevelTwo class, despite that LevelOne also requires string in its .ctor registered parameter should not be propagated into LevelOne");
    }

    [Test]
    public void should_use_parameter_value_for_all_build_plans()
    {
      const string expected = "value";

      // --arrange
      var target = CreateTarget();

      target.Treat<LevelOne>().AsIs();
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
      const string expected             = "value";
      const string expectedOnLevelThree = "levelThree";

      //--arrange
      var target = CreateTarget();

      target.Treat<LevelOne>().AsIs();
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

    [Test]
    public void dependency_should_use_personal_parameter_even_if_matching_path_is_shorter()
    {
      const string expected = "expected";

      //--arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>() // short matching path 
       .AsIs()
       .UsingParameters(expected);

      target
       .Treat<ISubject1>() // long matching path
       .AsCreated<LevelTwo>()
       .BuildingWhich(_ => _.TreatAll().UsingParameters(expected + "bad"));

      // --act
      var actual = (LevelTwo) target.Build<ISubject1>();

      // --assert
      actual.LevelOne.Text.Should().Be(expected, "Because {0} is registered for {1}", expected, typeof(LevelOne).Name);
    }

    [Test]
    public void should_inject_parameter_value_from_narrower_context_not_from_longer_matching_path()
    {
      const string levelThree = "levelThree";
      const string expected   = "expected";

      var target = CreateTarget();

      target.Treat<ISubject1>().AsCreated<LevelThree>().BuildingWhich(_ => _.TreatAll().UsingParameters(levelThree)); // longer path
      target.Treat<LevelTwo>().AsIs().BuildingWhich(_ => _.TreatAll().UsingParameters(expected));                     // narrower context
      target.Treat<LevelOne>().AsIs();

      var actual = target.Build<ISubject1>();

      actual.Should().BeOfType<LevelThree>().Which.LevelTwo.LevelOne.Text.Should().Be(expected);
    }

    [Test]
    public void should_inject_default_value_into_first_parameter()
    {
      const int expectedInt = LevelOne.DefaultInt - 1;

      //--arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingInjectPointConstructor(LevelOne.TwoParameterCtor)
       .UsingParameters(expectedInt);

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().Be(LevelOne.DefaultText);
      actual.Value.Should().Be(expectedInt);
    }

    [Test]
    public void should_inject_default_value_into_both_parameters()
    {
      //--arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingInjectPointConstructor(LevelOne.TwoParameterCtor);

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().Be(LevelOne.DefaultText);
      actual.Value.Should().Be(LevelOne.DefaultInt);
    }

    [Test]
    public void should_inject_parameter_value_from_narrower_context()
    {
      const string expected = "expected";

      //--arrange
      var target = CreateTarget();

      target
       .Treat<LevelThree>()
       .AsIs()
       .BuildingWhich(_ => _.TreatAll().UsingParameters(expected + "three"));

      target
       .Treat<LevelTwo>()
       .AsIs()
       .BuildingWhich(_ => _.TreatAll().UsingParameters(expected));

      target
       .Treat<LevelOne>()
       .AsIs();

      // --act
      var actual = target.Build<LevelThree>();

      // --assert
      actual.LevelTwo.LevelOne.Text.Should().Be(expected);
    }

    private static IEnumerable ForParameterSource()
    {
      yield return new TestCaseData(ForParameter.OfType<string>()).SetName("OfType");
      yield return new TestCaseData(ForParameter.Named("text")).SetName("Named");
      yield return new TestCaseData(ForParameter.WithInjectPoint(null)).SetName("WithInjectPoint");
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer
                 {
                   new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
                   GetLongestConstructorBuildAction
                    .Instance // constructor with largest number of parameters has less priority
                 }),
             new IfLastUnitMatches(MethodArgumentPattern.Instance)
              .UseBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer() {CreateParameterValueBuildAction.Instance, GetDefaultParameterValueBuildAction.Instance}) // autowiring
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

    private class LevelOne : ISubject1, ISubject2
    {
      public const string DefaultText = "defaultText";
      public const int    DefaultInt  = 39085;

      public const string TwoParameterCtor = "2";

      [Inject] // default ctor
      public LevelOne([Inject] string text) => Text = text;

      [Inject(TwoParameterCtor)]
      public LevelOne(string text = DefaultText, int value = DefaultInt)
      {
        Text  = text;
        Value = value;
      }

      public int    Value { get; }
      public string Text  { get; }
    }

    private class LevelTwo : LevelOne
    {
      public readonly LevelOne LevelOne;

      public LevelTwo(LevelOne levelOne, string text) : base(text) => LevelOne = levelOne;
    }

    private class LevelThree : LevelOne
    {
      public readonly LevelTwo LevelTwo;

      public LevelThree(LevelTwo levelTwo, string text) : base(text) => LevelTwo = levelTwo;
    }
  }
}
