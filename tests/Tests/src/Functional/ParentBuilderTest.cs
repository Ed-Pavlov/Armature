﻿using System;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

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
      const string value1   = "1";
      const string value2   = "2";
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
      const string value2   = "2";

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
      var target  = CreateTarget(parent1, parent2);

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
      var argumentOutOfRangeException = new ArgumentOutOfRangeException();
      var invalidProgramException     = new InvalidProgramException();

      // --arrange
      var parent1 = new Builder(BuildStage.Create)
       .With(builder => builder.Treat<string>().AsCreatedWith(() => throw argumentOutOfRangeException));

      var parent2 = new Builder(BuildStage.Create)
       .With(builder => builder.Treat<string>().AsCreatedWith(() => throw invalidProgramException));

      var target = CreateTarget(parent1, parent2);

      // --act
      Action action = () => target.Build<string>();

      // --assert
      action.Should()
            .Throw<AggregateException>()
            .Which
            .InnerExceptions.Should()
            .Equal(argumentOutOfRangeException, invalidProgramException);
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
      target
       .GetOrAddNode(new SkipAllUnits())
       .AddNode(new IfFirstUnit(new CanBeInstantiated()))
       .UseBuildAction(new DebugOnlyBuildAction(), BuildStage.Cache);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_catch_not_serializable_exception_when_building_via_parent()
    {
      var parent = CreateTarget();
      parent.Treat<Subject>().AsCreatedWith(() => throw new NotSerializableException());

      var target = CreateTarget(parent);

      // --act
      Action actual = () => target.Build<Subject>();

      // --assert
      actual.Should().ThrowExactly<AggregateException>().WithInnerExceptionExactly<NotSerializableException>();
    }

    private class DebugOnlyBuildAction : IBuildAction
    {
      public void Process(IBuildSession buildSession) { }

      public void PostProcess(IBuildSession buildSession) { }
    }

    private static Builder CreateTarget(params Builder[] parents)
      => new(new[] {BuildStage.Cache, BuildStage.Create}, parents)
         {
           new SkipAllUnits
           {
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create)
           }
         };

    private class Subject
    {
      public readonly string String;

      public Subject(string @string) => String = @string;
    }
  }
}