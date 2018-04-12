using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
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
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }
    
    private static Builder CreateTarget()
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(BuildStage.Create, GetLongesConstructorBuildAction.Instance)
      };

      var container = new Builder(new[] {BuildStage.Cache, BuildStage.Create});
      container.Children.Add(treatAll);
      return container;
    }
    
    private class Subject{}
  }
}