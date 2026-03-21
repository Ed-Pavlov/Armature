using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
using BeatyBit.Lifetimes;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.Functional
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

      target.Treat<Subject>().AsIs();

      // --act
      var actual = target.Build<Subject>()!;

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

      target.Treat<Subject>().AsIs();

      // --act
      var actual = target.Build<Subject>(expectedText, expectedValue)!;

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
      var actual = target.Build<Subject>(expectedValue)!;

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

      target.Treat<string>().AsInstance(null!);
      target.Treat<int>().AsInstance(expectedValue);

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>()!;

      // --assert
      actual.Text.Should().BeNull();
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_use_inject_point_id_as_tag()
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
      var actual = target.Build<Subject>()!;

      // --assert
      actual.Text.Should().Be(expectedText);
      actual.Value.Should().Be(expectedValue);
    }

    [Test]
    public void should_fail_if_there_is_no_value_wo_tag_registered()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>("tag")
       .AsInstance("09765");

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      Action actual = () => target.Build<Subject>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    private class OverrideDefaultUnitPattern : Default
    {
      public static void Override(Lifetime lifetime) => _ = new OverrideDefaultUnitPattern(lifetime);
      private OverrideDefaultUnitPattern(Lifetime lifetime)
        => lifetime.Bracket(
          () =>
          {
            var prevValue = CreateUnitPatternInTreatType;
            CreateUnitPatternInTreatType = (type, tag) => new IsAssignableFromType(type, tag);
            return prevValue;
          },
          prevValue => CreateUnitPatternInTreatType = prevValue);
    }

    [Test]
    public void should_wire_type_assignable_to_interface()
    {
      using var disposable = Lifetime.CreateDisposable();

      // --arrange
      var lifetime = (Lifetime) disposable;
      OverrideDefaultUnitPattern.Override(lifetime);

      var target = CreateTarget();
      target.Treat<Stream>().AsInstance(new MemoryStream());

      // --act
      var actual = target.Build<IDisposable>();

      // --assert
      actual.Should().BeOfType<MemoryStream>();
    }

    private static Builder CreateTarget()
      => new("test", BuildStage.Cache, BuildStage.Create)
         {
           // inject into constructor
           new IfFirstUnit(new IsConstructor())
            .UseBuildAction(
               new TryInOrder
               {
                 new GetConstructorByInjectPoint(null),     // constructor marked with [Inject] attribute has higher priority
                 new GetConstructorWithMaxParametersCount() // constructor with the largest number of parameters has lower priority
               },
               BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoArray())
            .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterArgument())
            .UseBuildAction(
               new TryInOrder {Static.Of<BuildArgumentByParameterInjectPoint>(), Static.Of<BuildArgumentByParameterType>()},
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

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
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
