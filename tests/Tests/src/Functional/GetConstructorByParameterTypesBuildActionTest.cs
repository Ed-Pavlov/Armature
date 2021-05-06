using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

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
       .UsingArguments(0, "0", new object())
       .InjectInto(Constructor.WithParameters(typeof(int), typeof(string)));

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
       .UsingArguments(0, "0", new object())
       .InjectInto(Constructor.Parameterless());

      // --act

      var actual = target.Build<Subject>();

      // --assert
      actual.ParameterlessCtorIsCalled.Should().BeTrue();
    }

    private static Builder CreateTarget()
      => new(BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(
                 new BuildActionChain
                 {
                   new GetConstructorByInjectPointId(), // constructor marked with [Inject] attribute has more priority
                   GetLongestConstructor
                    .Instance // constructor with largest number of parameters has less priority
                 },
                 BuildStage.Create)
           }
         };

    private class Subject
    {
      public readonly bool ExpectedCtorIsCalled;
      public readonly bool ParameterlessCtorIsCalled;

      public Subject() => ParameterlessCtorIsCalled = true;
      public Subject(int i, string s) => ExpectedCtorIsCalled = true;

      public Subject(string s, int i) { }

      [Inject]
      public Subject(int i, string s, object o) { }
    }
  }
}
