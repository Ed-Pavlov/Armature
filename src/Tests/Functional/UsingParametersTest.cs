using System;
using Armature;
using Armature.Core;
using Armature.Interface;
using Armature.Logging;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class UsingParametersTest
  {
    [Test]
    public void WithoutParameters()
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
    public void RegisterWithTwoWeakParameters()
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
        .ShouldBeEquivalentTo(new
          {
            String = expectedString,
            Disposable = expectedDisposable
          });
    }

    [Test]
    public void RegisterNullAsParameterValue()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<OneStringCtorClass>()
        .AsIs()
        .UsingParameters(For.Parameter<string>().UseValue(null));

      // --act
      var actual = target.Build<OneStringCtorClass>();

      // --assert
      actual.Text.Should().BeNull();
    }

    [Test]
    public void RegisterUsingDifferentBuildPlansWithDifferentParameters()
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
    public void ParametersShouldBeAppliedDirectlyToTypeTheyRegistiredFor()
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
    public void RegisterParametersForAll()
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
    public void WhenResolvingUnitTreatAll()
    {
      const string expectedString3 = "value";
      const string l3ExpectedString = "levelThree";

      using(Log.Enabled(LogLevel.Info))
      {
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
        actual.String.Should().Be(l3ExpectedString);
        actual.LevelTwo.String.Should().Be(expectedString3);
        actual.LevelTwo.LevelOne.String.Should().Be(expectedString3);
      }
    }

    [Test]
    public void WhenResolvingDependencyTreatAll()
    {
      const string expectedString3 = "value";
      const string l3ExpectedString = "levelThree";

      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<LevelOne>().AsIs();
      target.Treat<LevelTwo>().AsIs();
      target
        .Treat<LevelThree>()
        .AsIs()
        .UsingParameters(l3ExpectedString);

      target
        .Building<LevelTwo>()
        .TreatAll()
        .UsingParameters(expectedString3);

      // --act
      var actual = target.Build<LevelThree>();

      // --assert
      actual.String.Should().Be(l3ExpectedString);
      actual.LevelTwo.String.Should().Be(expectedString3);
      actual.LevelTwo.LevelOne.String.Should().Be(expectedString3);
    }

    [Test]
    public void NamedParameter()
    {
      const string expected = "expected";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(For.ParameterName("string").UseValue(expected));

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.String.Should().Be(expected);
    }

    [Test]
    public void MarkedParameter()
    {
      const string expected = "expected";

      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<LevelOne>()
        .AsIs()
        .UsingParameters(For.ParameterId(null).UseValue(expected));

      // --act
      var actual = target.Build<LevelOne>();

      // --assert
      actual.String.Should().Be(expected);
    }

    [UsedImplicitly]
    private class LevelOne
    {
      public readonly string String;

      public LevelOne([Inject]string @string)
      {
        String = @string;
      }
    }

    [UsedImplicitly]
    private class LevelTwo : LevelOne
    {
      public readonly LevelOne LevelOne;

      public LevelTwo(LevelOne levelOne, string @string) : base(@string)
      {
        LevelOne = levelOne;
      }
    }

    [UsedImplicitly]
    private class LevelThree : LevelOne
    {
      public readonly LevelTwo LevelTwo;

      public LevelThree(LevelTwo levelTwo, string @string) : base(@string)
      {
        LevelTwo = levelTwo;
      }
    }
  }
}