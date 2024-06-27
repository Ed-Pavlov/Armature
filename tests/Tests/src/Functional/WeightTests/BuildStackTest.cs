using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional.WeightTests
{
  public class BuildStackTest
  {
    [Test]
    public void should_use_registration_from_longer_branch()
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
      var actual = target.Build<ISubject>()!;

      // --assert
      actual.Value.Should().Be(expected);
    }

    private static Builder CreateTarget()
      => new("test", BuildStage.Cache, BuildStage.Create)
         {
                 // inject into constructor
                 new IfFirstUnit(new IsConstructor())
                    .UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),
                 new IfFirstUnit(new IsParameterInfoArray())
                    .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                 new IfFirstUnit(new IsParameter())
                    .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
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