using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.UnitMatchers;
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
        .Override<Subject>()
        .AsIs()
        .UsingParameters(10);

      builder.Build<Subject>().Should().BeOfType<Subject>().Which.Value.Should().Be(10);
    }

    private static Builder CreateTarget()
    {
      var treatAll = new AnyUnitSequenceMatcher
      {
        // inject into constructor
        new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
          .AddBuildAction(BuildStage.Create, GetLongesConstructorBuildAction.Instance),
      };

      var container = new Builder(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create);
      container.Children.Add(treatAll);
      return container;
    }
    
    private class Subject
    {
      public int Value { get; }

      public Subject(){}
      public Subject(int value) => Value = value;
    }
  }
}