using System;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional;

public class ContextTest
{
  [Test]
  public void should_respect_context_registrations1()
  {
    // --arrange
    var target = CreateTarget();

    var i = target.Build<I>();
    i.Subject.Value.Should().Be("B");
  }

  [Test]
  public void should_respect_context_registrations2()
  {
    // --arrange
    var target = CreateTarget();

    var a = target.Build<A>();
    a.B.Subject.Value.Should().Be("AB");
  }

  private static Builder CreateTarget()
  {
    var target = new Builder(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
                  {
                    // inject into constructor
                    new IfFirstUnit(new IsConstructor())
                     .UseBuildAction(
                        new TryInOrder
                        {
                          new GetConstructorByInjectPointId(),              // constructor marked with [Inject] attribute has more priority
                          Static.Of<GetConstructorWithMaxParametersCount>() // constructor with largest number of parameters has less priority
                        },
                        BuildStage.Create),
                    new IfFirstUnit(new IsParameterInfo())
                     .UseBuildAction(
                        new TryInOrder {Static.Of<BuildArgumentByParameterInjectPointId>(), Static.Of<BuildArgumentByParameterType>()},
                        BuildStage.Create),
                    new IfFirstUnit(new IsParameterInfoList())
                     .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                    new IfFirstUnit(new IsPropertyInfo())
                     .UseBuildAction(
                        new TryInOrder {new BuildArgumentByPropertyType()},
                        BuildStage.Create)
                  };

    target.Building<A>()
          .Treat<Subject>()
          .UsingArguments("A");

    target.Building<B>()
          .Treat<Subject>()
          .UsingArguments("B");

    target.Building<A>()
          .Building<B>()
          .Treat<Subject>()
          .UsingArguments("AB");

    target.Treat<A>().AsIs();
    target.Treat<B>().AsIs();
    target.Treat<I>().As<B>();
    target.Treat<Subject>().AsIs();
    return target;
  }


  private class Subject
  {
    public string Value { get; }
    public Subject(string value) => Value = value ?? throw new ArgumentNullException(nameof(value));
  }

  private interface I
  {
    Subject Subject { get; }
  }

  private class B : I
  {
    public Subject Subject { get; }
    public B(Subject subject) => Subject = subject ?? throw new ArgumentNullException(nameof(subject));
  }

  private class A
  {
    public B B { get; }
    public A(B b) => B = b ?? throw new ArgumentNullException(nameof(b));
  }
}