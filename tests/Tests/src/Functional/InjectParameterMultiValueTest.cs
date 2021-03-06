﻿using System;
using System.Collections.Generic;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class InjectParameterMultiValueTest
  {
    [Test]
    public void should_inject_multi_value_parameter()
    {
      const string expectedText = "text";
      var          expectedInt  = new[] {1, 2, 3};

      // --arrange
      var target = CreateTarget();

      target.Treat<Subject>().AsIs();
      target.Treat<int>().AsInstance(expectedInt[0]);
      target.Treat<int>().AsInstance(expectedInt[1]);
      target.Treat<int>().AsInstance(expectedInt[2]);
      target.Treat<string>().AsInstance(expectedText);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Str.Should().Be(expectedText);
      actual.Collection.Should().BeEquivalentTo(expectedInt);
    }

    [Test]
    public void should_fail_when_multi_value_registered_for_one_parameter()
    {
      const string expectedText = "text";
      var          expectedInt  = new[] {1, 2, 3};

      // --arrange
      var target = CreateTarget();

      target.Treat<Subject>().AsIs();
      target.Treat<int>().AsInstance(expectedInt[0]);
      target.Treat<int>().AsInstance(expectedInt[1]);
      target.Treat<int>().AsInstance(expectedInt[2]);
      target.Treat<string>().AsInstance(expectedText);
      target.Treat<string>().AsInstance("second value");

      // --act
      Action action = () => target.Build<Subject>();

      // --assert
      action.Should().Throw<ArmatureException>().And.Message.Should().Contain("Two or more building actions have the same weight");
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new AnyUnitSequenceMatcher
           {
             // inject into constructor
             new LastUnitSequenceMatcher(ConstructorMatcher.Instance)
              .AddBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer
                 {
                   new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
                   GetLongestConstructorBuildAction
                    .Instance // constructor with largest number of parameters has less priority
                 }),
             new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
              .AddBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer()
                 {
                   CreateParameterValueBuildAction.Instance, CreateParameterMultiValueToInjectBuildAction.Instance, GetDefaultParameterValueBuildAction.Instance
                 }) // autowiring
           }
         };

    private class Subject
    {
      public readonly string           Str;
      public readonly IEnumerable<int> Collection;

      public Subject(string str, IEnumerable<int> collection)
      {
        Str        = str;
        Collection = collection;
      }
    }
  }
}
