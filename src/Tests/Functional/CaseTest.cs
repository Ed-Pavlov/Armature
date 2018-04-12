using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Armature;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

// Resharper disable all

namespace Tests.Functional
{
  public class CaseTest
  {
    [Test]
    public void Building()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateTarget();
      target
        .Building<IDisposableValue1>()
        .Treat<IDisposable>()
        .CreatedBy(_ => new MemoryStream());

      target
        .Treat<IDisposableValue1>()
        .As<OneDisposableCtorClass>();

      // --act
      var actual = target.Build<IDisposableValue1>();

      // --assert
      Assert.That(actual.Disposable, Is.InstanceOf<MemoryStream>());
    }

    [Test(Description = "Type registered as instance should be resolved to this instance if interfaces treated as this type")]
    public void RegisterViaInterfaceAndAsInstance()
    {
      // --arrange
      var expected = new EmptyCtorClass();
      var target = FunctionalTestHelper.CreateTarget();
      target
        .Treat<IEmptyInterface1>()
        .As<EmptyCtorClass>(AddCreateBuildAction.No);
      target
        .Treat<IEmptyInterface2>()
        .As<EmptyCtorClass>(AddCreateBuildAction.No);
      target
        .Treat<EmptyCtorClass>()
        .AsInstance(expected);

      // --act
      var actual1 = target.Build<IEmptyInterface1>();
      var actual2 = target.Build<IEmptyInterface2>();

      // --assert
      Assert.That(actual1, Is.SameAs(expected));
      Assert.That(actual2, Is.SameAs(expected));
    }

    [Test(Description = "Inject ILogger<T> into class T")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void LoggerCase()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateTarget();
      target
        .Treat<string>()
        .CreatedBy(assembler => assembler.BuildSequence.First().Id.ToString());

      target
        .Treat<TwoDisposableStringCtorClass>()
        .AsIs()
        .UsingParameters(new MemoryStream());

      // --act
      var actual = target.Build<TwoDisposableStringCtorClass>();

      // --assert
      Assert.That(actual.String, Is.EqualTo(actual.GetType().ToString()));
    }

    [Test]
    public void UsingParametersTwiceOnSameImplementationTest()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateTarget();

      target.Treat<IDisposableValue1>()
        .As<OneDisposableCtorClass>()
        .UsingParameters(new MemoryStream());

      var expected = new MemoryStream();
      target.Treat<IDisposableValue2>()
        .As<OneDisposableCtorClass>()
        .UsingParameters(expected);

      // --act
      var instance = target.Build<IDisposableValue2>();

      // --assert
      Assert.That(instance.Disposable, Is.SameAs(expected));
    }

    [Test]
    public void SameClassForMultipleTokenAsSingleton()
    {
      const string token1 = "t1";

      var target = FunctionalTestHelper.CreateTarget();

      target.Treat<IDisposableValue1>().As<OneDisposableCtorClass>(AddCreateBuildAction.No);
      target.Treat<IDisposableValue2>(token1).As<OneDisposableCtorClass>(AddCreateBuildAction.No);
      target.Treat<OneDisposableCtorClass>().AsInstance(new OneDisposableCtorClass(null));

      var dep = target.Build<IDisposableValue1>();
      var dep1 = target.UsingToken(token1).Build<IDisposableValue2>();

      Assert.AreSame(dep, dep1);
    }

    [Test(Description = "Registration of some entity separated in several parts should work")]
    public void SeparatedRegistration()
    {
      var target = FunctionalTestHelper.CreateTarget();
      target
        .Treat<IDisposableValue1>()
        .As<OneDisposableCtorClass>();

      target
        .Treat<IDisposableValue2>()
        .As<OneDisposableCtorClass>();

      target
        .Treat<OneDisposableCtorClass>()
        .UsingParameters(new MemoryStream());

      target
        .Treat<OneDisposableCtorClass>()
        .AsSingleton();

      var actual1 = target.Build<IDisposableValue1>();
      var actual2 = target.Build<IDisposableValue2>();

      // --assert
      actual1.Should().BeOfType<OneDisposableCtorClass>();
      actual1.Should().BeSameAs(actual2);
    }

    private interface IEmptyInterface1
    {
    }

    private interface IEmptyInterface2
    {
    }

    private class EmptyCtorClass : IEmptyInterface1, IEmptyInterface2
    {
      private static int _counter = 1;
      private readonly int _id = _counter++;

      public override string ToString() { return _id.ToString(); }
    }

    private interface IDisposableValue1
    {
      IDisposable Disposable { get; }
    }

    private interface IDisposableValue2
    {
      IDisposable Disposable { get; }
    }

    private class OneDisposableCtorClass : IDisposableValue1, IDisposableValue2
    {
      private readonly IDisposable _disposable;

      public OneDisposableCtorClass(IDisposable disposable) { _disposable = disposable; }

      public IDisposable Disposable { get { return _disposable; } }
    }

    private class OneStringCtorClass : IDisposableValue1, IDisposableValue2
    {
      private readonly string _text;

      public OneStringCtorClass(string text) { _text = text; }

      public string Text { get { return _text; } }

      IDisposable IDisposableValue1.Disposable { get { throw new NotImplementedException(); } }

      IDisposable IDisposableValue2.Disposable { get { throw new NotImplementedException(); } }
    }

    private class TwoDisposableStringCtorClass : OneDisposableCtorClass
    {
      public readonly string String;

      public TwoDisposableStringCtorClass(IDisposable disposable, string @string) : base(disposable) { String = @string; }
    }

    private class Disposable : IDisposable
    {
      public void Dispose() { }
    }
  }
}