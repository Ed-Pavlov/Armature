using System;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local

namespace Tests.Functional
{
  public class CreationTest
  {
    [Test]
    public void should_use_default_strategy()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<ISubject1>()
       .As<Subject>()
       .CreatedByDefault();

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>();
    }

    [Test]
    public void should_use_reflection_strategy()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<ISubject1>()
       .As<Subject>()
       .CreatedByReflection();

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>();
    }

    [Test]
    public void should_add_default_creation_strategy()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs();

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().BeOfType<Subject>();
    }

    [Test]
    public void should_create_type_on_building_interface()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<ISubject1>()
       .AsCreated<Subject>();

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>();
    }

    [Test]
    public void should_use_factory_method()
    {
      var expected = new Subject();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsCreatedWith(_ => expected);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_autowire_value_into_factory_method()
    {
      var          expected       = new Subject();
      const string expectedString = "expected397";

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsCreatedWith<string>(
          value =>
          {
            value.Should().Be(expectedString);

            return expected;
          })
       .UsingArguments(expectedString);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_use_creation_strategy_registered_with_tag()
    {
      const string tag      = "tag";
      var          expected = new Subject();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs();

      target
       .Treat<Subject>(tag)
       .AsCreatedWith(_ => expected);


      // --act
      var actual = target.UsingTag(tag).Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_pass_tag_to_creation_strategy()
    {
      const string tag              = "tag";
      var          createdByFactory = new Subject();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<ISubject1>()
       .As<Subject>(tag);

      target
       .Treat<Subject>(tag)
       .AsIs();

      target
       .Treat<Subject>()
       .AsCreatedWith(_ => createdByFactory);

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>().And.NotBeSameAs(createdByFactory);
    }

    [Test]
    public void should_use_instance()
    {
      var expected = new Subject();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsInstance(expected);

      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().BeSameAs(expected);
    }

    [Test]
    public void should_use_runtime_parameters_when_build_with_tag()
    {
      const string tag      = "tag";
      const int    expected = 98347;

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>(tag)
       .AsIs()
       .UsingInjectionPoints(Constructor.WithParameters<int>());

      // --act
      var actual = target.UsingTag(tag).Build<Subject>(expected)!;

      // --assert
      actual.Value.Should().Be(expected);
    }

    private static Builder CreateTarget() =>

        // builder
        //  .TreatAll()
        //  .InjectInto(Constructor.Parameterless())
        //  .UsingArguments(AutoBuildByParameter.Type);
        //
        // return builder;
        new("test", BuildStage.Cache, BuildStage.Create)
                                             {
                                                     new SkipTillUnit(new IsInheritorOf(typeof(IDisposable), null))
                                                        .UseBuildAction(new CreateByReflection(), BuildStage.Cache),

                                                     new IfFirstUnit(new IsConstructor())
                                                        .UseBuildAction(new GetConstructorByParameterTypes(), BuildStage.Create), // use empty ctor by default in this test

                                                     new IfFirstUnit(new IsParameterInfoArray())
                                                        .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
                                                     new IfFirstUnit(new IsParameterInfo())
                                                        .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create)
                                             };

    private interface ISubject1 { }

    private interface ISubject2 { }

    private class Subject : ISubject1, ISubject2
    {
      public readonly int Value;

      public Subject() { }

      public Subject(int value) => Value = value;
    }
  }
}