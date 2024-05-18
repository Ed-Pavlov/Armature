using System;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

#pragma warning disable 8618
#pragma warning disable 8602

namespace Tests.Functional
{
  public class ChoosingConstructorTest
  {
    [TestCaseSource(nameof(max_number_of_parameters))]
    public void should_call_ctor_with_max_number_of_parameters(IBuildAction getConstructorAction)
    {
      var target = new Builder("test", BuildStage.Cache, BuildStage.Create)
                   {
                       new IfFirstUnit(new IsConstructor()).UseBuildAction(getConstructorAction, BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
                   };

      // --arrange
      target.Treat<Subject2>()
            .AsIs()
            .UsingArguments(new object()); // set value to inject into ctor

      // --act
      var actual = target.Build<Subject2>();

      // --assert
      actual.LongestPublicConstructorIsCalled.Should().BeTrue();
    }

    [TestCaseSource(nameof(parameterless_ctor))]
    public void should_call_parameterless_ctor(IBuildAction getConstructorAction)
    {
      var target = new Builder("test", BuildStage.Create)
                   {
                       new IfFirstUnit(new IsConstructor()).UseBuildAction(getConstructorAction, BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
                   };

      // --arrange
      target
       .Treat<Subject2>()
       .AsIs()
       .UsingArguments(new object()); // set value to inject into ctor

      // --act
      var actual = target.Build<Subject2>();

      // --assert
      actual.ParameterlessConstructorIsCalled.Should().BeTrue();
    }

    [TestCaseSource(nameof(inject_point_id_ctor))]
    public void should_call_constructor_marked_with_attribute(string injectPointId, IBuildAction getConstructorAction)
    {
      var target = new Builder("test", BuildStage.Cache, BuildStage.Create)
                   {
                       new IfFirstUnit(new IsConstructor()).UseBuildAction(getConstructorAction, BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
                   };

      // --arrange
      target.Treat<Subject1>()
            .AsIs()
            .UsingArguments(new object()); // set value to inject into ctor

      // --act
      var actual = target.Build<Subject1>();

      // --assert
      actual.CalledConstructorPointId.Should().Be(injectPointId);
    }

    [Test]
    public void should_call_constructor_matched_with_parameter_types()
    {
      var target = new Builder("test", BuildStage.Cache, BuildStage.Create)
                   {
                       new IfFirstUnit(new IsConstructor()).UseBuildAction(
                         new TryInOrder(
                           new GetConstructorByInjectPoint(),
                           new GetConstructorWithMaxParametersCount(),
                           new GetConstructorByParameterTypes()
                         ),
                         BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
                   };

      // --arrange
      target.Treat<Subject3>()
            .AsIs()
            .UsingInjectionPoints(Constructor.WithParameters<int, string>())
            .UsingArguments(4, "string"); // set value to inject into ctor

      target
         .TreatInheritorsOf<object>()
         .UsingInjectionPoints(Constructor.WithMaxParametersCount());

      target
         .TreatInheritorsOf<IStream>()
         .AmendWeight(39)
         .UsingInjectionPoints(Constructor.Parameterless());

      // --act
      var actual = target.Build<Subject3>();

      // --assert
      actual.IntStringConstructorIsCalled.Should().BeTrue();
    }

    [Test]
    public void should_use_personal_rules_for_each_type()
    {
      var target = new Builder("test", BuildStage.Cache, BuildStage.Create)
                   {
                       new IfFirstUnit(new IsConstructor()).UseBuildAction(
                         new TryInOrder(
                           new GetConstructorByInjectPoint(),
                           new GetConstructorWithMaxParametersCount(),
                           new GetConstructorByParameterTypes()
                         ),
                         BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
                   };

      // --arrange
      target.Treat<object>().AsInstance(new object());

      target.Treat<Subject1>().AsIs().UsingInjectionPoints(Constructor.WithParameters<Subject3, Subject2>());
      target.Treat<Subject2>().AsIs().UsingInjectionPoints(Constructor.Parameterless());
      target.Treat<Subject3>().AsIs(); // use default rules, in this case the constructor with max parameters count

      // --act
      var actual = target.Build<Subject1>();

      // --assert
      actual.Subj2.ParameterlessConstructorIsCalled.Should().BeTrue();
      actual.Subj3.LongestPublicConstructorIsCalled.Should().BeTrue();
    }

    [Test]
    public void should_fail_if_no_constructor_found()
    {
      var target = new Builder("test", BuildStage.Cache, BuildStage.Create)
                   {
                       new IfFirstUnit(new IsConstructor()).UseBuildAction(
                         new TryInOrder(
                           new GetConstructorByInjectPoint(),
                           new GetConstructorWithMaxParametersCount(),
                           new GetConstructorByParameterTypes()
                         ),
                         BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                       new IfFirstUnit(new IsParameterInfo()).UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
                   };

      // --arrange
      target.Treat<bool>().AsIs();

      // --act
      Action action = () => target.Build<bool>();

      // --assert
      action.Should().Throw<ArmatureException>().And.Message.Should().StartWith($"Constructor for type {typeof(Boolean)} is not found, check registrations for this type or");
    }

    private static IEnumerable max_number_of_parameters()
    {
      yield return new TestCaseData(new GetConstructorWithMaxParametersCount()).SetName("WithMaxParametersCount");

      yield return new TestCaseData(
        new TryInOrder {new GetConstructorByInjectPoint(), new GetConstructorWithMaxParametersCount(), new GetConstructorByParameterTypes(),}
      ).SetName("ByInjectPointId -> WithMaxParametersCount -> Parameterless");
    }

    private static IEnumerable parameterless_ctor()
    {
      yield return new TestCaseData(new GetConstructorByParameterTypes()).SetName("Parameterless");

      yield return new TestCaseData(
        new TryInOrder {new GetConstructorByInjectPoint(), new GetConstructorByParameterTypes(), new GetConstructorWithMaxParametersCount(),}
      ).SetName("ByInjectPointId -> Parameterless -> WithMaxParametersCount");
    }

    private static IEnumerable inject_point_id_ctor()
    {
      yield return new TestCaseData(
        null,
        new TryInOrder {new GetConstructorByInjectPoint(), new GetConstructorByParameterTypes(), new GetConstructorWithMaxParametersCount(),}
      ).SetName("PointId = null");

      const string pointId = Subject1.InjectPointId;

      yield return new TestCaseData(
        pointId,
        new TryInOrder {new GetConstructorByInjectPoint(pointId), new GetConstructorByParameterTypes(), new GetConstructorWithMaxParametersCount(),}
      ).SetName($"PointId = {pointId}");
    }

    [UsedImplicitly]
    private class Subject2
    {
      public readonly bool LongestPublicConstructorIsCalled;
      public readonly bool ParameterlessConstructorIsCalled;

      public Subject2() => ParameterlessConstructorIsCalled = true;

      public Subject2(object _1) { }

      public Subject2(object _1, object _2) => LongestPublicConstructorIsCalled = true;

      protected Subject2(object _1, object _2, object _3) { }

      private Subject2(object _1, object _2, object _3, object _4) { }
    }

    [UsedImplicitly]
    private class Subject1
    {
      public const    string   InjectPointId = nameof(Subject1) + nameof(InjectPointId);

      public string? CalledConstructorPointId = nameof(CalledConstructorPointId) + "bad";

      public readonly Subject2 Subj2;
      public readonly Subject3 Subj3;

      [Inject(InjectPointId)]
      public Subject1() => CalledConstructorPointId = InjectPointId;

      [Inject]
      public Subject1(object _1) => CalledConstructorPointId = null;

      public Subject1(Subject2  _2, Subject3 _3) { }
      public Subject1(Subject3  _3, Subject2 _2) // initialize fields only in this ctor - indicates it was called, not other
      {
        Subj2 = _2;
        Subj3     = _3;
      }
      protected Subject1(object _1, object   _2, object _3) { }
      private Subject1(object   _1, object   _2, object _3, object _4) { }
    }

    [UsedImplicitly]
    private class Subject3
    {
      public readonly bool IntStringConstructorIsCalled;
      public readonly bool LongestPublicConstructorIsCalled;

      public Subject3() { }
      public Subject3(int    i,  string s) => IntStringConstructorIsCalled = true;
      public Subject3(string s,  int    i) { }
      public Subject3(object _1, object _2, object _3) => LongestPublicConstructorIsCalled = true;
    }
  }
}