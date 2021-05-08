using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable All

namespace Tests.Functional
{
  public class GetInjectPointConstructorBuildActionTest
  {
    [Test]
    public void should_call_inject_point_constructor()
    {
      // --arrange
      var container = CreateTarget();

      container
       .Treat<Subject>()
       .AsIs()
       .UsingArguments(new object()); // set value to inject into ctor

      // --act
      var instance = container.Build<Subject>();

      // --assert
      instance.InjectPointConstructorIsCalled.Should().BeTrue();
    }

    [Test]
    public void should_call_inject_point_constructor_with_specified_id()
    {
      // --arrange
      var container = CreateTarget();

      container
       .Treat<Subject>()
       .AsIs()
       .InjectInto(Constructor.MarkedWithInjectAttribute(Subject.InjectPointId));

      // --act
      var instance = container.Build<Subject>();

      // --assert
      instance.InjectPointWithIdConstructorIsCalled.Should().BeTrue();
    }

    [Test]
    public void should_not_use_attributed_ctor_for_dependency()
    {
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .UsingArguments("value");

      target
       .Treat<LevelTwo>()
       .AsIs()
       .InjectInto(Constructor.MarkedWithInjectAttribute(Subject.InjectPointId));

      var actual = target.Build<LevelTwo>();

      // --assert
      actual.Should().NotBeNull();
      actual.Dependency.Should().NotBeNull();
      actual.Dependency.InjectPointWithIdConstructorIsCalled.Should().BeFalse();
    }

    private Builder CreateTarget()
      => new(BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnit(IsConstructor.Instance)
              .UseBuildAction(
                 new TryInOrder
                 {
                   new GetConstructorByInjectPointId(), // constructor marked with [Inject] attribute has more priority
                   GetConstructorWithMaxParametersCount.Instance       // constructor with largest number of parameters has less priority
                 },
                 BuildStage.Create),
             new IfLastUnit(IsParameterInfo.Instance)
              .UseBuildAction(BuildArgumentByParameterType.Instance, BuildStage.Create)
           }
         };

    class Subject
    {
      public const    string InjectPointId = "int";
      public readonly bool   InjectPointConstructorIsCalled;

      public readonly bool InjectPointWithIdConstructorIsCalled;

      [Inject(InjectPointId)]
      public Subject()
      {
        InjectPointWithIdConstructorIsCalled = true;
      }

      [Inject]
      public Subject(object _1)
      {
        InjectPointConstructorIsCalled = true;
      }

      public Subject(object _1, object _2) { }
    }

    private class LevelTwo
    {
      public readonly Subject Dependency;

      [Inject(Subject.InjectPointId)]
      public LevelTwo(Subject dependency)
      {
        Dependency = dependency;
      }

      public LevelTwo(string value, int digit) // longest constructor
      { }
    }
  }
}
