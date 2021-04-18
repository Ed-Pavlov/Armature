using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable all

namespace Tests.Functional
{
  public class CachingTest
  {
    [Test]
    public void should_create_instance_only_once()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs()
       .AsSingleton();

      // --act
      var expected = target.Build<Subject>();
      var actual   = target.Build<Subject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance)
           }
         };

    private class Subject { }
  }
}
