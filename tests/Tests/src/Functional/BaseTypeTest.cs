using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

//Resharper disable all

namespace Tests.Functional
{
  public class BaseTypeTest
  {
    [Test]
    public void should_inject_dependency_into_all_inheritors_of_class()
    {
      const int expected = 39;

      // --arrange
      var target = CreateTarget();

      target.Treat<int>().AsInstance(expected);

      target
        .TreatInheritorsOf<SubjectBase>()
        .InjectProperty(Property.Named(nameof(SubjectBase.InjectThere)));

      target
        .Treat<Subject>()
        .AsIs();
      // --act
      var actual = target.Build<Subject>();

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
        .InjectProperty(Property.Named(nameof(ISubject.InjectThere)));

      target
        .Treat<Subject>()
        .AsIs();

      // --act
      var actual = target.Build<Subject>();

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
        .InjectProperty(Property.Named(nameof(ISubject.InjectThere)));

      target
        .Treat<Subject>()
        .AsIs()
        .InjectProperty(Property.Named(nameof(Subject.InjectHere)));
      
      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.InjectHere.Should().Be(expected);
      actual.InjectThere.Should().Be(expected);
    }

    private static Builder CreateTarget() =>
      new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
      {
        new AnyUnitSequenceMatcher
        {
          // inject into constructor
          new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
            .AddBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance), // constructor with largest number of parameters has less priority

          new LastUnitSequenceMatcher(PropertyValueMatcher.Instance)
            .AddBuildAction(BuildStage.Create, new CreatePropertyValueBuildAction())
        }
      };

    private interface ISubject
    {
      int InjectThere { get; set; }
    }

    private abstract class SubjectBase
    {
      public int InjectThere { get; set; }
    }

    private class Subject : SubjectBase, ISubject
    {
      public int InjectHere { get; set; }
    }
  }
}