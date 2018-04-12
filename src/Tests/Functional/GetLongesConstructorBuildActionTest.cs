using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable All

namespace Tests.Functional
{
  public class GetLongesConstructorBuildActionTest
  {
    [Test]
    public void should_call_ctor_with_largest_number_of_parameters()
    {
      var target = CreateTarget();

      // --arrange
      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameters(new object()); // set value to inject into ctor

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.ExpectedConstructorIsCalled.Should().BeTrue();
    }

    private static Builder CreateTarget()
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(
            BuildStage.Create,
            new OrderedBuildActionContainer
            {
              new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
              GetLongesConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
            }),
      };

      var container = new Builder(new[]{ BuildStage.Create});
      container.Children.Add(treatAll);
      return container;
    }

    private class Subject
    {
      public readonly bool ExpectedConstructorIsCalled;
      
      public Subject(){}
      public Subject(object _1){}
      public Subject(object _1, object _2) => ExpectedConstructorIsCalled = true;
      protected Subject(object _1, object _2, object _3){}
      private Subject(object _1, object _2, object _3, object _4){}
    }
  }
}