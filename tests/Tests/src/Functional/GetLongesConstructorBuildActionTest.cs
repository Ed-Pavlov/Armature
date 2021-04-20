﻿using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable All

namespace Tests.Functional
{
  public class GetLongesConstructorBuildActionTest
  {
    [Test]
    public void should_call_ctor_with_largest_number_of_parameters()
    {
      var target = CreateTarget();

      // --arrange
      target
       .Treat<Subject>()
       .AsIs()
       .UsingMethodArguments(new object()); // set value to inject into ctor

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.ExpectedConstructorIsCalled.Should().BeTrue();
    }

    [Test]
    public void should_not_fail_if_no_constructors()
    {
      var target = CreateTarget();

      // --arrange
      target.Treat<bool>().AsIs();

      // --act
      Action action = () => target.Build<bool>();

      // --assert
      action.Should().Throw<Exception>().And.Message.Should().Be("Can't find appropriate constructor for type System.Boolean");
    }

    private static Builder CreateTarget()
      => new(BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(
                 new OrderedBuildActionContainer
                 {
                   new GetConstructorByInjectPointId(), // constructor marked with [Inject] attribute has more priority
                   GetLongestConstructor
                    .Instance // constructor with largest number of parameters has less priority
                 },
                 BuildStage.Create)
           }
         };

    private class Subject
    {
      public readonly bool ExpectedConstructorIsCalled;

      public Subject() { }

      public Subject(object _1) { }

      public Subject(object _1, object _2) => ExpectedConstructorIsCalled = true;

      protected Subject(object _1, object _2, object _3) { }

      private Subject(object _1, object _2, object _3, object _4) { }
    }
  }
}
