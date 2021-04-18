using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
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
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance)
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
