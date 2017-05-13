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
      var target = FunctionalTestHelper.CreateContainer();

      target
        .TreatOpenGeneric(typeof(IGeneric<>))
        .As(typeof(Generic<>));

      var actual = target.Build<IGeneric<int>>();
      actual.Should().BeOfType<Generic<int>>();
    }

    [Test]
    public void RegisterWithUsingParameters()
    {
      var target = FunctionalTestHelper.CreateContainer();

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
      var target = FunctionalTestHelper.CreateContainer();

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
      public T Value { get{throw new NotSupportedException();} }
    }

    private class GenericWithParameter<T> : IGeneric<T>
    {
      private readonly T _value;

      public GenericWithParameter(T value)
      {
        _value = value;
      }

      public T Value
      {
        get { return _value; }
      }

      public override string ToString()
      {
        return string.Format("{0}", _value);
      }
    }
  }
}