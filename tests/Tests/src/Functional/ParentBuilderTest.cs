using System;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.Common;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class ParentBuilderTest
  {
    [Test]
    public void should_try_build_value_via_all_parent_builders()
    {
      const string expected = "2";

      // --arrange
      var parent1 = CreateTarget();
      var parent2 = CreateTarget();
      parent2
        .Treat<string>()
        .AsInstance(expected);

      var target = CreateTarget(parent1, parent2);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected, "Expected value registered in one of the parent build (passed last for testing purpose)");
    }

    [Test]
    public void should_build_value_via_child_builder_first()
    {
      const string value1 = "1";
      const string value2 = "2";
      const string expected = "325";

      // --arrange
      var parent1 = CreateTarget();
      parent1
        .Treat<string>()
        .AsInstance(value1);

      var parent2 = CreateTarget();
      parent2
        .Treat<string>()
        .AsInstance(value2);

      var target = CreateTarget(parent1, parent2);
      target
        .Treat<string>()
        .AsInstance(expected);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected, "Expected value registered in child builder");
    }

    [Test]
    public void should_build_value_via_parent_builders_in_order()
    {
      const string expected = "1";
      const string value2 = "2";

      // --arrange
      var parent1 = CreateTarget();
      parent1
        .Treat<string>()
        .AsInstance(expected);

      var parent2 = CreateTarget();
      parent2
        .Treat<string>()
        .AsInstance(value2);

      var target = CreateTarget(parent1, parent2);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected, "Expected value registered in the parent builder which passed first");
    }

    [Test]
    public void should_fail_if_there_is_no_registration_in_any_builder()
    {
      // --arrange
      var parent1 = CreateTarget();
      var parent2 = CreateTarget();
      var target = CreateTarget(parent1, parent2);

      // --act
      Action action = () => target.Build<string>();

      // --assert
      action.Should().ThrowExactly<ArmatureException>("There is no registration neither in child neither in parent builders");
    }

    [Test]
    public void should_build_parameter_value_via_parent()
    {
      const string expected = "expectedstring";

      // --arrange
      var parentBuilder = new Builder(BuildStage.Cache);
      parentBuilder.Treat<string>().AsInstance(expected);

      var target = CreateTarget(parentBuilder);
      target
        .Treat<Subject>()
        .AsIs();
      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.String.Should().Be(expected);
    }

    [Test]
    public void should_use_parameter_value_from_local_build_plan()
    {
      const string expected = "expectedstring";

      // --arrange
      var parentBuilder = new Builder(BuildStage.Cache);
      parentBuilder.Treat<string>().AsInstance(expected + "bad");

      var target = CreateTarget(parentBuilder);
      target
        .Treat<string>()
        .AsInstance(expected);

      target
        .Treat<Subject>()
        .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().NotBeNull();
      actual.String.Should().Be(expected);
    }

    [Test]
    public void should_report_all_exceptions_from_parent_builders()
    {
      var parent1 = new Builder(BuildStage.Create)
        .With(builder => builder.Treat<string>().AsCreatedWith(() => throw new ArgumentOutOfRangeException()));
      
      var parent2 = new Builder(BuildStage.Create)
        .With(builder => builder.Treat<string>().AsCreatedWith(() => throw new InvalidProgramException()));

      var target = CreateTarget(parent1, parent2);

      Action action = () => target.Build<string>();

      action.Should()
        .Throw<ArmatureException>()
        .Which
        .Data.Values.Cast<object>()
        .With(_ => _.SingleOrDefault(exc => exc is ArgumentOutOfRangeException).Should().NotBeNull())
        .With(_ => _.SingleOrDefault(exc => exc is InvalidProgramException).Should().NotBeNull());
    }

    [Test]
    public void should_not_throw_exception_if_one_parent_built_unit()
    {
      const string expected = "parent2string";
      
      var parent1 = new Builder(BuildStage.Create)
        .With(builder => builder.Treat<string>().AsCreatedWith(() => throw new ArgumentOutOfRangeException()));

      var parent2 = new Builder(BuildStage.Cache)
        .With(builder => builder.Treat<string>().AsInstance(expected));

      var target = CreateTarget(parent1, parent2);

      var actual = target.Build<string>();

      actual.Should().Be(expected);
    }

    [Test]
    public void should_build_via_parent_if_build_action_exists_but_did_not_built_result()
    {
      const string expected = "parent2string";
      
      var parent = new Builder(BuildStage.Cache)
        .With(builder => builder.Treat<string>().AsInstance(expected));

      var target = CreateTarget(parent);
      
      // add build action which actual doesn't build any value, in this case Armature should try to build an unit via parent builder 
      target.AddOrGetUnitSequenceMatcher(new AnyUnitSequenceMatcher())
        .Add(new LastUnitSequenceMatcher(AnyTypeMatcher.Instance)
          .AddBuildAction(BuildStage.Cache, new DebugOnlyBuildAction() ));

      // --act
      var actual = target.Build<string>();
      
      // --assert
      actual.Should().Be(expected);
    }

    private class DebugOnlyBuildAction : IBuildAction
    {
      public void Process(IBuildSession buildSession){}

      public void PostProcess(IBuildSession buildSession)
      {
        
      }
    }
    
    private static Builder CreateTarget(params Builder[] parents) =>
      new Builder(new[] {BuildStage.Cache, BuildStage.Create}, parents)
      {
        new AnyUnitSequenceMatcher
        {
          new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
            .AddBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance),
          new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
            .AddBuildAction(BuildStage.Create, CreateParameterValueBuildAction.Instance)
        }
      };

    private class Subject
    {
      public readonly string String;

      public Subject(string @string) => String = @string;
    }
  }
}