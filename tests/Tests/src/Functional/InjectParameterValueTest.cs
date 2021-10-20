﻿using System;
using System.Collections;
using Armature;
using Armature.Core;
using Armature.Core.Logging;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

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
       .Treat<string>() //TODO: may be Treat should generate IfLastUnit? why should we perform know to be false matching? Building<> should generate SkipTillUnit 
       .AsInstance(expected + "bad");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(expected);

      target.PrintToLog();

      using var hz = Log.Enabled(LogLevel.Trace);
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
       .InjectInto(Constructor.MarkedWithInjectAttribute(LevelOne.TwoParameterCtor))
       .UsingArguments(expectedInt, expectedString);

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
       .UsingArguments(asInterfaceParameterValue);

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(asIsParameterValue);

      using var _ = Log.Enabled(LogLevel.Verbose);
      
      var asInterface = target.Build<ISubject1>();
      Console.WriteLine("///////////////////////////////");
      Console.WriteLine("///////////////////////////////");
      Console.WriteLine("///////////////////////////////");
      var asIs        = target.Build<LevelOne>();

      // --assert
      asInterface.Text.Should().Be(asInterfaceParameterValue);
      asIs.Text.Should().Be(asIsParameterValue);
    }

    [TestCaseSource("ForParameterSource")]
    public void should_pass_null_as_parameter_value(MethodArgumentTuner forParameter)
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
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().BeNull();
    }

    [TestCaseSource("ForParameterSource")]
    public void should_use_value_for_parameter(MethodArgumentTuner forParameter)
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
      var actual = target.Build<LevelOne>();

      // --assert
      actual.Text.Should().Be(expected);
    }

    [TestCaseSource("ForParameterSource")]
    public void should_build_value_for_parameter_using_parameter_type_and_key(MethodArgumentTuner forParameter)
    {
      const string key      = "key398";
      const string expected = "expected 398752";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseKey(key), "bad");

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
    public void should_fail_if_there_is_no_value_w_key_registered(MethodArgumentTuner forParameter)
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>()
       .AsInstance("09765");

      target
       .Treat<LevelOne>()
       .AsIs()
       .UsingArguments(forParameter.UseKey("key"));

      // --act
      Action actual = () => target.Build<LevelOne>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [TestCaseSource("ForParameterSource")]
    public void should_use_factory_method(MethodArgumentTuner forParameter)
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
       .UsingArguments(expected);

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

      var tuner = target
                 .Treat<LevelOne>()
                 .AsIs();

      // --act
      Action actual = () => tuner.UsingArguments(
                        ForParameter.OfType<string>().UseKey("expected29083"),
                        ForParameter.OfType<string>().UseValue("kldj"));

      // --assert
      actual.Should()
            .ThrowExactly<ArmatureException>()
            .Where(_ => _.Message.StartsWith($"Building of an argument for the method parameter of type {typeof(string).ToLogString()} is already tuned"));
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

      target
       .TreatAll()
       .UsingArguments(expected);

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
       .UsingArguments(expectedOnLevelThree);

      target
       .Building<LevelThree>()
       .TreatAll()
       .UsingArguments(expected);

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
       .UsingArguments(expected);

      target
       .Treat<ISubject1>() // long matching path
       .AsCreated<LevelTwo>()
       .BuildingWhich(_ => _.TreatAll().UsingArguments(expected + "bad"));

      Console.WriteLine("Main Tree");
      target.PrintToLog();
      Console.WriteLine("");
      Console.WriteLine("//////////////////////////////////////");

      using var _ = Log.Enabled(LogLevel.Verbose);

      // --act
      var actual = (LevelTwo)target.Build<ISubject1>();

      // --assert
      actual.LevelOne.Text.Should().Be(expected, "Because {0} is registered for {1}", expected, typeof(LevelOne).Name);
    }

    [Test]
    public void should_inject_parameter_value_from_narrower_context_not_from_longer_matching_path()
    {
      const string levelThree = "levelThree";
      const string expected   = "expected";

      var target = CreateTarget();

      // target.Treat<ISubject1>().AsCreated<LevelThree>();
      // target.Building<ISubject1>().Building<LevelThree>().TreatAll().UsingArguments(levelThree); // longer path
      // target.Treat<LevelTwo>().AsIs().BuildingWhich(_ => _.TreatAll().UsingArguments(expected)); // narrower context
      // target.Treat<LevelOne>().AsIs();
      
      target.Treat<ISubject1>().AsCreated<LevelThree>().BuildingWhich(_ => _.TreatAll().UsingArguments(levelThree)); // longer path
      target.Treat<LevelTwo>().AsIs().BuildingWhich(_ => _.TreatAll().UsingArguments(expected));                     // narrower context
      target.Treat<LevelOne>().AsIs();

      using var _ = Log.Enabled(LogLevel.Trace);

      var actual = target.Build<ISubject1>(); //TODO: почему финальный вес 50? откуда берётся такое большое число?

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
       .InjectInto(Constructor.MarkedWithInjectAttribute(LevelOne.TwoParameterCtor))
       .UsingArguments(expectedInt);

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
       .InjectInto(Constructor.MarkedWithInjectAttribute(LevelOne.TwoParameterCtor));

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
       .BuildingWhich(_ => _.TreatAll().UsingArguments(expected + "three"));

      target
       .Treat<LevelTwo>()
       .AsIs()
       .BuildingWhich(_ => _.TreatAll().UsingArguments(expected));

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
      yield return new TestCaseData(ForParameter.OfType(typeof(string))).SetName("OfType");
      yield return new TestCaseData(ForParameter.Named("text")).SetName("Named");
      yield return new TestCaseData(ForParameter.WithInjectPoint(null)).SetName("WithInjectPoint");
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipAllUnits
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
                 new TryInOrder() { Static<BuildArgumentByParameterType>.Instance, Static<GetParameterDefaultValue>.Instance },
                 BuildStage.Create)
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
