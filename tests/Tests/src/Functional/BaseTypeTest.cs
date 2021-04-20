using Armature;
using Armature.Core;
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
       .UsingPropertyArguments(Property.Named(nameof(SubjectBase.InjectThere)));

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
       .UsingPropertyArguments(Property.Named(nameof(ISubject.InjectThere)));

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
       .UsingPropertyArguments(Property.Named(nameof(ISubject.InjectThere)));

      target
       .Treat<Subject>()
       .AsIs()
       .UsingPropertyArguments(Property.Named(nameof(Subject.InjectHere)));

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.InjectHere.Should().Be(expected);
      actual.InjectThere.Should().Be(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(GetLongestConstructor.Instance, BuildStage.Create),

             new IfLastUnitMatches(PropertyArgumentPattern.Instance)
              .UseBuildAction(new BuildArgumentForProperty(), BuildStage.Create)
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
