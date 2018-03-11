using System;
using Armature;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class OpenGenericTest
  {
    [Test]
    public void Register()
    {
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .TreatOpenGeneric(typeof(IGeneric<>))
        .As(typeof(Generic<>));

      target
        .Treat<IGeneric<int>>()
        .As<Generic<int>>();

      var actual = target.Build<IGeneric<int>>();
      actual.Should().BeOfType<Generic<int>>();
    }

    [Test]
    public void RegisterWithUsingParameters()
    {
      var target = FunctionalTestHelper.CreateBuilder();

      const int expected = 5;
      target
        .TreatOpenGeneric(typeof(IGeneric<>))
        .As(typeof(GenericWithParameter<>))
        .UsingParameters(expected);

      var actual = target.Build<IGeneric<int>>();
      actual.Value.Should().Be(expected);
    }

    [Test]
    public void RegisterAsOpenAndClosedGeneric()
    {
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .TreatOpenGeneric(typeof(IGeneric<>))
        .As(typeof(GenericWithParameter<>))
        .UsingParameters("open");

      const string closed = "closed";
      target
        .Treat<IGeneric<string>>()
        .As<GenericWithParameter<string>>()
        .UsingParameters(closed);

      var actual = target.Build<IGeneric<string>>();
      actual.Value.Should().Be(closed);
    }

    private interface IGeneric<out T>
    {
      T Value { get; }
    }

    private class Generic<T> : IGeneric<T>
    {
      public T Value => throw new NotSupportedException();
    }

    private class GenericWithParameter<T> : IGeneric<T>
    {
      public GenericWithParameter(T value) => Value = value;

      public T Value { get; }

      public override string ToString() => string.Format("{0}", Value);
    }
  }
}