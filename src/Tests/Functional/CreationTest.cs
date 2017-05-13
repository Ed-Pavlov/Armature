using Armature;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class CreationTest
  {
    [Test(Description = "Type registered AsIs w/o type redirection")]
    public void AsIs()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<EmptyCtorClass>()
        .AsIs();

      // --act
      var actual = target.Build<EmptyCtorClass>();

      // --assert
      actual.Should().BeOfType<EmptyCtorClass>();
    }

    [Test(Description = "Treat interface as concrete type")]
    public void As()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<IEmptyInterface1>()
        .As<EmptyCtorClass>();

      // --act
      var actual = target.Build<IEmptyInterface1>();

      // --assert
      actual.Should().BeOfType<EmptyCtorClass>();
    }

    [Test]
    public void CreatedByMethodFactory()
    {
      // --arrange
      EmptyCtorClass expected = null;
      var target = FunctionalTestHelper.CreateContainer();
      target
        .Treat<EmptyCtorClass>()
        .CreatedBy(_ =>
        {
          expected = new EmptyCtorClass();
          return expected;
        });

      // --act
      var actual = target.Build<EmptyCtorClass>();

      // --assert
      actual.Should().Be(expected);
    }

    /// <summary>
    /// <see cref="TreatSugar{T}.As{TRedirect}(object, AddCreationBuildStep)"/> adds default creation strategy
    /// </summary>
    [Test]
    public void AsAddsDefaultCreationStrategy()
    {
      var target = FunctionalTestHelper.CreateContainer();

      target
        .Treat<IEmptyInterface1>()
        .As<EmptyCtorClass>()
        .AsSingleton();

      target
        .Treat<IEmptyInterface2>()
        .As<EmptyCtorClass>()
        .AsSingleton();

      target
        .Treat<EmptyCtorClass>()
        .AsIs()
        .AsSingleton();

      var actual1 = target.Build<IEmptyInterface1>();
      var actual2 = target.Build<IEmptyInterface2>();
      var actual3 = target.Build<EmptyCtorClass>();

      // --assert
      actual1.Should().NotBe(actual2);
      actual1.Should().NotBe(actual3);
      actual2.Should().NotBe(actual3);
    }

    [Test]
    public void AsWithoutDefaultCreationStrategy()
    {
      var target = FunctionalTestHelper.CreateContainer();

      target
        .Treat<IEmptyInterface1>()
        .As<EmptyCtorClass>(AddCreationBuildStep.No)
        .AsSingleton();

      target
        .Treat<IEmptyInterface2>()
        .As<EmptyCtorClass>(AddCreationBuildStep.No)
        .AsSingleton();

      target
        .Treat<EmptyCtorClass>()
        .AsIs()
        .AsSingleton();

      var actual1 = target.Build<IEmptyInterface1>();
      var actual2 = target.Build<IEmptyInterface2>();
      var actual3 = target.Build<EmptyCtorClass>();

      // --assert
      actual1.Should().Be(actual2);
      actual1.Should().Be(actual3);
    }
  }
}