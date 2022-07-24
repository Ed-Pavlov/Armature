using System;
using System.Collections;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class InjectParameterValueTest
  {
    [Test]
    public void should_autowire_values_passed_into_using_arguments_rather_then_registered()
    {
      const string expected = "expected 09765";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>()
       .AsInstance(expected + "bad");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(expected);

      // --act
      var actual = target.Build<LevelOne>()!;

      // --assert
      actual.Text.Should().Be(expected);
    }

    [Test]
    public void should_pass_two_values()
    {
      // --arrange
      const int    expectedInt    = 389;
      const string expectedString = "expected";

      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingInjectionPoints(Constructor.MarkedWithInjectAttribute(LevelOne.TwoParameterCtor))
       .UsingArguments(expectedInt, expectedString);

      // --act
      var actual = target.Build<LevelOne>()!;

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
       .UsingArguments(asInterfaceParameterValue);

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(asIsParameterValue);

      var asInterface = target.Build<ISubject1>()!;
      var asIs        = target.Build<LevelOne>()!;

      // --assert
      asInterface.Text.Should().Be(asInterfaceParameterValue);
      asIs.Text.Should().Be(asIsParameterValue);
    }

    [TestCaseSource(nameof(ForParameterSource))]
    public void should_pass_null_as_parameter_value(MethodArgumentTuner<object?> forParameter)
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>()
       .AsInstance("938754");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseValue(null));

      // --act
      var actual = target.Build<LevelOne>()!;

      // --assert
      actual.Text.Should().BeNull();
    }

    [TestCaseSource(nameof(ForParameterSource))]
    public void should_use_value_for_parameter(MethodArgumentTuner<object?> forParameter)
    {
      const string expected = "expected";

      // --arrange
      var target = CreateTarget();

      target.Treat<string>().AsInstance("bad");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseValue(expected), "bad");

      // --act
      var actual = target.Build<LevelOne>()!;

      // --assert
      actual.Text.Should().Be(expected);
    }

    [TestCaseSource(nameof(ForParameterSource))]
    public void should_build_value_for_parameter_using_parameter_type_and_tag(MethodArgumentTuner<object?> forParameter)
    {
      const string tag      = "tag398";
      const string expected = "expected 398752";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseTag(tag), "bad");

      target
       .Treat<string>(tag)
       .AsInstance(expected);

      target
       .Treat<string>("bad_tag")
       .AsInstance("bad");

      target
       .Treat<string>()
       .AsInstance("bad");

      // --act
      var actual = target.Build<LevelOne>()!;

      // --assert
      actual.Text.Should().Be(expected);
    }

    [TestCaseSource(nameof(ForParameterSource))]
    public void should_fail_if_there_is_no_value_w_tag_registered(MethodArgumentTuner<object?> forParameter)
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>()
       .AsInstance("09765");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseTag("tag"));

      // --act
      Action actual = () => target.Build<LevelOne>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [TestCaseSource(nameof(ForParameterSource))]
    public void should_use_factory_method(MethodArgumentTuner<object?> forParameter)
    {
      const int expectedInt = 392;

      // --arrange
      var target = CreateTarget();

      target.Treat<int>().AsInstance(expectedInt);

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseFactoryMethod<int>(intValue => intValue.ToString()));

      // --act
      var actual = target.Build<LevelOne>()!;

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
       .UsingArguments(expected);

      // --act
      var actual = target.Build<LevelOne>("bad")!;

      // --assert
      actual.Text.Should().Be(expected);
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
       .UsingArguments(expected);

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

      target.Treat<string>().AsInstance(expected);

      // --act
      var actual = target.Build<LevelThree>()!;

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
       .UsingArguments(expectedOnLevelThree);

      target
       .Building<LevelThree>()
       .TreatAll()
       .UsingArguments(expected);

      target.PrintToLog();

      // --act
      var actual = target.Build<LevelThree>()!;

      // --assert
      actual.Text.Should().Be(expectedOnLevelThree, "Because {0} is registered for {1}", expectedOnLevelThree, nameof(LevelThree));
      actual.LevelTwo.Text.Should().Be(expected, "Because {0} is registered for all {1} dependencies", expected, nameof(LevelThree));
      actual.LevelTwo.LevelOne.Text.Should().Be(expected, "Because {0} is registered for all {1} dependencies", expected, nameof(LevelThree));
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
       .UsingArguments(expected);

      target
       .Treat<ISubject1>() // long matching path
       .AsCreated<LevelTwo>();

      target.Building<LevelTwo>() .TreatAll().UsingArguments(expected + "bad");

      // --act
      var actual = (LevelTwo)target.Build<ISubject1>()!;

      // --assert
      actual.LevelOne.Text.Should().Be(expected, "Because {0} is registered for {1}", expected, nameof(LevelOne));
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
       .UsingInjectionPoints(Constructor.MarkedWithInjectAttribute(LevelOne.TwoParameterCtor))
       .UsingArguments(expectedInt);

      // --act
      var actual = target.Build<LevelOne>()!;

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
       .UsingInjectionPoints(Constructor.MarkedWithInjectAttribute(LevelOne.TwoParameterCtor));

      // --act
      var actual = target.Build<LevelOne>()!;

      // --assert
      actual.Text.Should().Be(LevelOne.DefaultText);
      actual.Value.Should().Be(LevelOne.DefaultInt);
    }


    private static IEnumerable ForParameterSource()
    {
      yield return new TestCaseData(ForParameter.OfType(typeof(string))).SetName("OfType");
      yield return new TestCaseData(ForParameter.Named("text")).SetName("Named");
      yield return new TestCaseData(ForParameter.WithInjectPoint(null)).SetName("WithInjectPoint");
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
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
             new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(
                 new TryInOrder() { Static.Of<BuildArgumentByParameterType>(), Static.Of<GetParameterDefaultValue>() },
                 BuildStage.Create)
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
      [UsedImplicitly]
      public LevelOne(string text = DefaultText, int value = DefaultInt)
      {
        Text  = text;
        Value = value;
      }

      public int    Value { get; }
      public string Text  { get; }
    }

    [UsedImplicitly]
    private class LevelTwo : LevelOne
    {
      public readonly LevelOne LevelOne;

      public LevelTwo(LevelOne levelOne, string text) : base(text) => LevelOne = levelOne;
    }

    [UsedImplicitly]
    private class LevelThree : LevelOne
    {
      public readonly LevelTwo LevelTwo;

      public LevelThree(LevelTwo levelTwo, string text) : base(text) => LevelTwo = levelTwo;
    }
  }
}