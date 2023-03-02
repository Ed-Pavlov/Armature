using System.Collections.Generic;
using Armature;
using Armature.BuildActions.Constructor;
using Armature.BuildActions.Method;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.UnitPatterns;
using Armature.UnitPatterns.Method;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class TreatOpenGenericTest
  {
    // [Test]
    public void test()
    {
      // --arrange
      var target = CreateTarget();

      target.TreatOpenGeneric(typeof(List<>))
            .AsCreated<List<int>>();

      var actual = target.BuildUnit(Unit.Of(typeof(List<>)));
    }

    [Test]
    public void should_instantiate_type_with_specified_generic_parameter()
    {
      // --arrange
      var target = CreateTarget();

      target
       .TreatOpenGeneric(typeof(ISubject<>))
       .AsCreated(typeof(Subject<>));

      target
       .Treat<ISubject<int>>()
       .AsCreated<Subject<int>>()
       .UsingInjectionPoints(Constructor.Parameterless());

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
       .AsCreated(typeof(Subject<>))
       .UsingArguments(expected);

      // --act
      var actual = target.Build<ISubject<int>>()!;

      // --assert
      actual.Value.Should().Be(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
             new IfFirstUnit(new IsConstructor()) // inject into constructor
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),

             new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),

             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
         };

    private interface ISubject<out T>
    {
      T? Value { get; }
    }

    private class Subject<T> : ISubject<T>
    {
      [UsedImplicitly]
      public Subject() { }

      public Subject(T value) => Value = value;

      public T? Value { get; }

      public override string ToString() => string.Format("{0}", Value);
    }
  }
}