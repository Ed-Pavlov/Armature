using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitSequenceMatcher;
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
    public void should_use_value_matched_with_longest_sequence_and_token()
    {
      object token = "token";
      const int expected = 9345;
      // --arrange
      var target = CreateTarget();

      target.Treat<ISubject>(token).AsCreated<Subject>(token);

      target.Treat<int>().AsInstance(5);

      target
        .Building<Subject>(token)
        .Treat<int>()
        .AsInstance(354);

      target
        .Building<ISubject>(token)
        .Building<Subject>()
        .Treat<int>()
        .AsInstance(986);

      target
        .Building<ISubject>(token)
        .Building<Subject>(token)
        .Treat<int>()
        .AsInstance(expected);

      // --act
      var actual = target.UsingToken(token).Build<ISubject>();

      // --assert
      actual.Value.Should().Be(expected);
    }

    private static Builder CreateTarget() =>
      new Builder(BuildStage.Cache, BuildStage.Create)
      {
        new AnyUnitSequenceMatcher
        {
          // inject into constructor
          new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
            .AddBuildAction(BuildStage.Create, GetLongesConstructorBuildAction.Instance),

          new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
            .AddBuildAction(BuildStage.Create, CreateParameterValueBuildAction.Instance)
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