using System;
using Armature;
using Armature.Common;
using Armature.Core;
using Armature.Interface;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class UsingParametersTest
  {
    [Test]
    public void should_throw_exception_if_there_is_no_value_for_parameter()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<OneStringCtorClass>()
        .AsIs();

      Action build = () => target.Build<OneStringCtorClass>();

      // --act, assert
      build.ShouldThrowExactly<ArmatureException>();
    }

    [Test]
    public void should_inject_provided_values_to_constructor()
    {
      var expectedDisposable = new Disposable();
      const string expectedString = "dsljf";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<TwoDisposableStringCtorClass>()
        .AsIs()
        .UsingParameters(expectedDisposable, expectedString);

      // --act
      var actual = target.Build<TwoDisposableStringCtorClass>();

      // --assert
      actual
        .ShouldBeEquivalentTo(
          new
          {
            String = expectedString,
            Disposable = expectedDisposable
          });
    }

    [Test]
    public void should_register_null_as_parameter_value()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<OneStringCtorClass>()
        .AsIs()
        .UsingParameters(ForParameter.OfType<string>().UseValue(null));

      // --act
      var actual = target.Build<OneStringCtorClass>();

      // --assert
      actual.Text.Should().BeNull();
    }

    [Test]
    public void should_register_different_parameter_values_in_different_build_plans()
    {
      var target = FunctionalTestHelper.CreateBuilder();

      const string asInterfaceParameterValue = "AsInterface";
      const string asIsParameterValue = "AsIs";
      target
        .Treat<IDisposableValue1>()
        .As<OneStringCtorClass>()
        .UsingParameters(asInterfaceParameterValue);

      target
        .Treat<OneStringCtorClass>()
        .AsIs()
        .UsingParameters(asIsParameterValue);

      var asInterface = (OneStringCtorClass)target.Build<IDisposableValue1>();
      var asIs = target.Build<OneStringCtorClass>();

      // --assert
      asInterface.Text.Should().Be(asInterfaceParameterValue);
      asIs.Text.Should().Be(asIsParameterValue);
    }

    [Test]
    public void should_not_pass_registered_parameter_when_building_dependency()
    {
      // Register string parameter only for LevelTwo class, despite that LevelOne also requires string in its .ctor
      // registered parameter should not be propagated into LevelOne

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<LevelOne>()
        .AsIs();

      const string expectedString = "value";
      target
        .Treat<LevelTwo>()
        .AsIs()
        .UsingParameters(expectedString);

      Action actual = () => target.Build<LevelTwo>();
      actual.ShouldThrow<ArmatureException>("Because there is no string registered for LevelOne");
    }

    [Test]
    public void should_use_parameter_value_for_all_build_plans()
    {
      const string expectedString = "value";

      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<LevelOne>().AsIs();
      target.Treat<LevelTwo>().AsIs();
      target.Treat<LevelThree>().AsIs();

      target
        .TreatAll()
        .UsingParameters(expectedString);
      var actual = target.Build<LevelThree>();

      // --assert
      actual.String.Should().Be(expectedString);
      actual.LevelTwo.String.Should().Be(expectedString);
      actual.LevelTwo.LevelOne.String.Should().Be(expectedString);
    }

    [Test]
    public void should_use_one_parameter_for_unit_and_other_for_dependencies()
    {
      const string expectedString3 = "value";
      const string l3ExpectedString = "levelThree";

      //--arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<LevelOne>().AsIs();
      target.Treat<LevelTwo>().AsIs();
      target
        .Treat<LevelThree>()
        .AsIs()
        .UsingParameters(l3ExpectedString);

      target
        .Building<LevelThree>()
        .TreatAll()
        .UsingParameters(expectedString3);

      // --act
      var actual = target.Build<LevelThree>();

      // --assert
      actual.String.Should().Be(l3ExpectedString, "Because {0} is registered for {1}", l3ExpectedString, typeof(LevelThree).Name);
      actual.LevelTwo.String.Should().Be(expectedString3, "Because {0} is registered for all {1} dependencies", expectedString3, typeof(LevelThree).Name);
      actual.LevelTwo.LevelOne.String.Should().Be(expectedString3, "Because {0} is registered for all {1} dependencies", expectedString3, typeof(LevelThree).Name);
    }

    [Test]
    public void should_use_value_for_parameter_with_given_name()
    {
      const string expected = "expected";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(ForParameter.Named("string").UseValue(expected));

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.String.Should().Be(expected);
    }

    [Test]
    public void should_use_value_for_parameter_marked_with_attribute()
    {
      const string expected = "expected";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(ForParameter.WithInjectPoint(null).UseValue(expected));

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.String.Should().Be(expected);
    }

    [Test]
    public void should_build_value_for_parameter_using_parameter_type_and_token()
    {
      const string rightToken = "token398";
      const string badToken = "sdoy7256";
      const string expected = "expected 398752";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();
      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(ForParameter.OfType<string>().UseToken(rightToken));

      target
        .Treat<string>(rightToken)
        .AsInstance(expected);

      target
        .Treat<string>(badToken)
        .AsInstance(expected + "dlskjgflkj");

      target
        .Treat<string>()
        .AsInstance("sldfjk lkjsd sdf ");

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.String.Should().Be(expected);
    }

    [Test]
    public void should_use_resolver_if_value_is_not_provided()
    {
      const int expectedInt = 392;

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();
      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(ForParameter.OfType<string>().UseResolver<int>((_, intValue) => intValue.ToString()));

      // --act
      var actual = target.Build<LevelOne>(expectedInt);

      // --assert
      actual.String.Should().Be(expectedInt.ToString());
    }

    [Test]
    public void should_use_personal_parameter_value_but_runtime_parameter()
    {
      const string expected = "expected29083";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();
      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(ForParameter.OfType<string>().UseValue(expected));

      // --act
      var actual = target.Build<LevelOne>(expected + "bad");

      // --assert
      actual.String.Should().Be(expected);
    }
    
    [Test]
    public void should_fail_if_value_for_the_same_parameter_registered_more_than_once()
    {
      const string expected = "expected29083";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      var adjusterSugar = target
        .Treat<LevelOne>()
        .AsIs();
      
      // --act
      Action actual = () =>adjusterSugar .UsingParameters(ForParameter.OfType<string>().UseToken(expected), ForParameter.OfType<string>().UseValue("kldj"));
      
      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }

    [UsedImplicitly]
    private class LevelOne
    {
      public readonly string String;

      public LevelOne([Inject] string @string) => String = @string;
    }

    [UsedImplicitly]
    private class LevelTwo : LevelOne
    {
      public readonly LevelOne LevelOne;

      public LevelTwo(LevelOne levelOne, string @string) : base(@string) => LevelOne = levelOne;
    }

    [UsedImplicitly]
    private class LevelThree : LevelOne
    {
      public readonly LevelTwo LevelTwo;

      public LevelThree(LevelTwo levelTwo, string @string) : base(@string) => LevelTwo = levelTwo;
    }
  }
}