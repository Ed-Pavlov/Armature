using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Armature;
using Armature.BuildActions.Constructor;
using Armature.BuildActions.Property;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.UnitPatterns;
using Armature.UnitPatterns.Property;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class BaseTypeTest
  {
    private int megaValue;
    public void test()
    {
      var builder = CreateTarget();

      builder
       .Treat<IStream>()
       .As<MemoryStream>()
       .CreatedByReflection()
       .UsingArguments(
          3,
          ForParameter.OfType<string>().UseTag("myStream"),
          ForParameter.WithInjectPoint(Tag.MegaDependency).UseValue(megaValue),
          ForProperty.Named("Prop").UseFactoryMethod(() => "value"))
       .UsingInjectionPoints(
          Constructor.WithMaxParametersCount())
       .AsSingleton()
       .BuildingIt()
       .Treat<IDisposable[]>()
       .AsCreatedWith(bs => (IDisposable[]) bs.BuildAllUnits(Unit.Of(typeof(IDisposable))).Select(_ => _.Entity.Value).ToArray())
       .UsingArguments("arg1", "arg2");
    }

    class Tag
    {
      public const int MegaDependency = 0;
    }

    [Test]
    public void should_inject_dependency_into_all_inheritors_of_class()
    {
      const int expected = 39;

      // --arrange
      var target = CreateTarget();

      target.Treat<int>().AsInstance(expected);

      target
       .TreatInheritorsOf<SubjectBase>()
       .UsingInjectionPoints(Property.Named(nameof(SubjectBase.InjectThere)));

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>()!;

      // --assert
      actual.InjectThere.Should().Be(expected);
    }

    [Test]
    public void should_inject_dependency_into_all_inheritors_of_interface()
    {
      const int expected = 39;

      // --arrange
      var target = CreateTarget();

      target.Treat<int>().AsInstance(expected);

      target
       .TreatInheritorsOf<ISubject>()
       .UsingInjectionPoints(Property.Named(nameof(ISubject.InjectThere)));

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>()!;

      // --assert
      actual.InjectThere.Should().Be(expected);
    }

    [Test]
    public void should_inject_dependency_all_specified_properties()
    {
      const int expected = 39;

      // --arrange
      var target = CreateTarget();

      target.Treat<int>().AsInstance(expected);

      target
       .TreatInheritorsOf<ISubject>()
       .UsingInjectionPoints(Property.Named(nameof(ISubject.InjectThere)));

      target
       .Treat<Subject>()
       .AsIs()
       .UsingInjectionPoints(Property.Named(nameof(Subject.InjectHere)));

      // --act
      var actual = target.Build<Subject>()!;

      // --assert
      actual.InjectHere.Should().Be(expected);
      actual.InjectThere.Should().Be(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           // inject into constructor
           new IfFirstUnit(new IsConstructor())
            .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),

           new IfFirstUnit(new IsPropertyInfo())
            .UseBuildAction(new BuildArgumentByPropertyType(), BuildStage.Create)
         };

    private interface ISubject
    {
      int InjectThere { get; set; }
    }

    private abstract class SubjectBase
    {
      public int InjectThere { get; set; }
    }

    [UsedImplicitly]
    private class Subject : SubjectBase, ISubject
    {
      public int InjectHere { get; set; }
    }
  }
}
