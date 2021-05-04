using Armature;
using Armature.Core;
using Armature.Core.Logging;
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

      using var _ = Log.Enabled(LogLevel.Verbose);
      // --act
      var actual = target.Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void creation_build_action_should_be_added_only_once()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<ISubject1>()
       .As<Subject>();

      target
       .Treat<ISubject2>()
       .As<Subject>();

      target
       .Treat<Subject>()
       .AsIs()
       .AsSingleton();

      // --act
      var actual1 = target.Build<ISubject1>();
      var actual2 = target.Build<ISubject2>();
      var actual3 = target.Build<Subject>();

      // --assert
      actual1.Should().BeSameAs(actual2);
      actual1.Should().BeSameAs(actual3);
    }

    [Test]
    public void should_use_creation_strategy_registered_with_key()
    {
      const string key      = "key";
      var          expected = new Subject();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>()
       .AsIs();

      target
       .Treat<Subject>(key)
       .AsCreatedWith(_ => expected);


      // --act
      var actual = target.UsingKey(key).Build<Subject>();

      // --assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_pass_key_to_creation_strategy()
    {
      const string key              = "key";
      var          createdByFactory = new Subject();

      // --arrange
      var target = CreateTarget();

      target
       .Treat<ISubject1>()
       .AsCreated<Subject>(key);

      target
       .Treat<Subject>(key)
       .AsIs();

      target
       .Treat<Subject>()
       .AsCreatedWith(_ => createdByFactory);

      // --act
      var actual = target.Build<ISubject1>();

      // --assert
      actual.Should().BeOfType<Subject>().And.Should().NotBeSameAs(createdByFactory);
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
    public void should_use_runtime_parameters_when_build_with_key()
    {
      const string key      = "key";
      const int    expected = 98347;

      // --arrange
      var target = CreateTarget();

      target
       .Treat<Subject>(key)
       .AsIs()
       .UsingConstructorWithParameters<int>();

      // --act
      var actual = target.UsingKey(key).Build<Subject>(expected);

      // --assert
      actual.Value.Should().Be(expected);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(new GetConstructorByParameterTypes(), BuildStage.Create) // use empty ctor by default in this test
           }
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
