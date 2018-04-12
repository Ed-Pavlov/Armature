using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class GetConstructorByParameterTypesBuildActionTest
  {
    [Test]
    public void should_call_ctor_with_corresponding_parameters()
    {
      // --arrange
      var target = CreateTarget();

      target
        .Treat<Subject>()
        .AsIs()
        .UsingConstructorWithParameters(typeof(int), typeof(string))
        .UsingParameters(0, "0", new object());
      
      // --act
      var actual = target.Build<Subject>();
      
      // --assert
      actual.ExpectedCtorIsCalled.Should().BeTrue();
    }
    
    [Test]
    public void should_call_parameterless_ctor()
    {
      // --arrange
      var target = CreateTarget();

      target
        .Treat<Subject>()
        .AsIs()
        .UsingParameterlessConstructor()
        .UsingParameters(0, "0", new object());
      
      // --act
      var actual = target.Build<Subject>();
      
      // --assert
      actual.ParameterlessCtorIsCalled.Should().BeTrue();
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
      public readonly bool ParameterlessCtorIsCalled;
      public readonly bool ExpectedCtorIsCalled;

      public Subject() => ParameterlessCtorIsCalled = true;
      public Subject(int i, string s) => ExpectedCtorIsCalled = true;
      public Subject(string s, int i) {}
      [Inject]
      public Subject(int i, string s, object o) {}
    }
  }
}