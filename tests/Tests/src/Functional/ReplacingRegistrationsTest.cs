using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

namespace Tests.Functional
{
  public class ReplacingRegistrationsTest
  {
    [Test]
    public void ReplaceSingleton()
    {
      var builder = CreateTarget();

      builder
       .Treat<Subject>()
       .AsIs()
       .UsingParameterlessConstructor();

      builder
       .OverrideTreat<Subject>()
       .AsIs()
       .UsingParameters(10);

      builder.Build<Subject>().Should().BeOfType<Subject>().Which.Value.Should().Be(10);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new AnyUnitSequenceMatcher
           {
             // inject into constructor
             new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
              .AddBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance),
             new LastUnitSequenceMatcher(ParametersArrayMatcher.Instance)
              .AddBuildAction(BuildStage.Create, CreateParametersArrayBuildAction.Instance)
           }
         };

    private class Subject
    {
      public Subject() { }

      public Subject(int value) => Value = value;
      public int Value { get; }
    }
  }
}
