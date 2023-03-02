using Armature;
using Armature.BuildActions.Constructor;
using Armature.Core;
using Armature.UnitPatterns;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class SingletonTest
  {
    [Test]
    public void should_create_singleton()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<Subject>()
            .AsIs()
            .AsSingleton();

      // --act
      var expected = target.Build<Subject>();
      var actual   = target.Build<Subject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }

    [Test]
    public void should_create_singleton_via_interface()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<ISubject>()
            .AsCreated<Subject>()
            .AsSingleton();

      // --act
      var expected = target.Build<ISubject>();
      var actual   = target.Build<ISubject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }

    [Test]
    public void should_create_singleton_if_interface_is_singleton()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<ISubject>()
            .AsSingleton();

      target.Treat<ISubject>()
            .AsCreated<Subject>();

      // --act
      var expected = target.Build<ISubject>();
      var actual   = target.Build<ISubject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
             new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create)
         };

    private interface ISubject { }

    [UsedImplicitly]
    private class Subject : ISubject { }
  }
}