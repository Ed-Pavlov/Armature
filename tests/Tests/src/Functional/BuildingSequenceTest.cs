using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class BuildingSequenceTest
  {
    [Test]
    public void should_use_value_matched_with_longest_sequence()
    {
      const int expected = 9345;

      // --arrange
      var target = CreateTarget();

      target.Treat<ISubject>().AsCreated<Subject>();

      target.Treat<int>().AsInstance(5);

      target
       .Building<Subject>()
       .Treat<int>()
       .AsInstance(354);

      target
       .Building<ISubject>()
       .Building<Subject>()
       .Treat<int>()
       .AsInstance(expected);

      // --act
      var actual = target.Build<ISubject>();

      // --assert
      actual.Value.Should().Be(expected);
    }

    [Test]
    public void should_use_value_matched_with_longest_sequence_and_key()
    {
      object    key      = "key";
      const int expected = 9345;

      // --arrange
      var target = CreateTarget();

      target.Treat<ISubject>(key).AsCreated<Subject>(key);

      target.Treat<int>().AsInstance(5);

      target
       .Building<Subject>(key)
       .Treat<int>()
       .AsInstance(354);

      target
       .Building<ISubject>(key)
       .Building<Subject>()
       .Treat<int>()
       .AsInstance(986);

      target
       .Building<ISubject>(key)
       .Building<Subject>(key)
       .Treat<int>()
       .AsInstance(expected);

      // --act
      var actual = target.UsingKey(key).Build<ISubject>();

      // --assert
      actual.Value.Should().Be(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(GetLongestConstructor.Instance, BuildStage.Create),
             new IfLastUnitMatches(MethodArgumentPattern.Instance)
              .UseBuildAction(BuildArgumentForMethodParameter.Instance, BuildStage.Create)
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
