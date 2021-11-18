using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional.WeightTests
{
  public class BuildChainTest
  {
    [Test]
    public void should_throw_exception_on_ambiguous_registrations()
    {
      const int expected = 9345;

      // --arrange
      var target = CreateTarget();

      target.Treat<ISubject>().AsCreated<Subject>();

      target.Treat<int>().AsInstance(expected - 1);

      target
         .Building<Subject>()
         .Treat<int>()
         .AsInstance(expected + 1);

      target
         .Building<ISubject>()
         .Building<Subject>()
         .Treat<int>()
         .AsInstance(expected);

      // --act
      var actual = new Action(() => target.Build<ISubject>());

      // --assert
      actual.Should()
            .ThrowExactly<ArmatureException>()
            .Where(_ => _.Message.StartsWith("Two or more building actions matched with the same weight"));
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
             new SkipAllUnits
             {
                 // inject into constructor
                 new IfFirstUnit(new IsConstructor())
                    .UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
                 new IfFirstUnit(new IsParameterInfoList())
                    .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                 new IfFirstUnit(new IsParameterInfo())
                    .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
             }
         };

    private interface ISubject
    {
      int Value { get; }
    }

    private class Subject : ISubject
    {
      public Subject(int value) => Value = value;
      public int Value { get; }
    }
  }
}