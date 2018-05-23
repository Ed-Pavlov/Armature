using System;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

namespace Tests.Functional
{
  public class TreatOpenGenericTest
  {
    [Test]
    public void should_instantiate_type_with_specified_generic_parameter()
    {
      // --arrange
      var target = CreateTarget();

      target
        .TreatOpenGeneric(typeof(ISubject<>))
        .As(typeof(Subject<>));

      target
        .Treat<ISubject<int>>()
        .As<Subject<int>>()
        .UsingParameterlessConstructor();

      // --act
      var actual = target.Build<ISubject<int>>();
      
      // --assert
      actual.Should().BeOfType<Subject<int>>();
    }

    [Test]
    public void should_pass_value_to_ctor_parameter()
    {
      // --arrange
      var target = CreateTarget();

      const int expected = 5;
      target
        .TreatOpenGeneric(typeof(ISubject<>))
        .As(typeof(Subject<>))
        .UsingParameters(expected);

      // --act
      var actual = target.Build<ISubject<int>>();
      
      // --assert
      actual.Value.Should().Be(expected);
    }

    [Test]
    public void closed_generic_registration_should_have_advantage_over_open_generic()
    {
      // --arrange
      var target = CreateTarget();

      target
        .TreatOpenGeneric(typeof(ISubject<>))
        .As(typeof(Subject<>))
        .UsingParameters("open");

      const string closed = "closed";
      target
        .Treat<ISubject<string>>()
        .As<Subject<string>>()
        .UsingParameters(closed);

      // --act
      var actual = target.Build<ISubject<string>>();
      
      // --assert
      actual.Value.Should().Be(closed);
    }

    [Test]
    public void should_not_add_creation_strategy()
    {
      // --arrange
      var target = CreateTarget();

      target
        .TreatOpenGeneric(typeof(ISubject<>))
        .As(typeof(Subject<>), AddCreateBuildAction.No);

      // --act
      Action actual = () => target.Build<ISubject<int>>(5);
      
      // --assert
      actual.Should().Throw<ArmatureException>();
    }
    
    private static Builder CreateTarget() => 
      new Builder(BuildStage.Create)
    {
      new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(BuildStage.Create, GetLongesConstructorBuildAction.Instance)
      }
    };

    private interface ISubject<out T>
    {
      T Value { get; }
    }

    private class Subject<T> : ISubject<T>
    {
      public Subject(){}

      public Subject(T value) => Value = value;

      public T Value { get; }

      public override string ToString() => string.Format("{0}", Value);
    }
  }
}