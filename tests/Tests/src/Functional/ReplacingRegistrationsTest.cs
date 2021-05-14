using Armature;
using Armature.Core;
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
       .InjectInto(Constructor.Parameterless());

      builder
       .TreatOverride<Subject>()
       .AsIs()
       .UsingArguments(10);

      builder.Build<Subject>().Should().BeOfType<Subject>().Which.Value.Should().Be(10);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             new IfLastUnit(new IsConstructor()) // inject into constructor
              .UseBuildAction(Static<GetConstructorWithMaxParametersCount>.Instance, BuildStage.Create),
             
             new IfLastUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             
             new IfLastUnit(new IsParameterInfo())
              .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
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
