using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

// Resharper disable all

namespace Tests.Functional
{
  public class CaseTest
  {
    [Test(Description = "Inject ILogger<T> into class T")]
    public void LoggerCase()
    {
      // --arrange
      var target = CreateTarget();

      target
       .Treat<string>()
       .AsCreatedWith(assembler => assembler.BuildSequence.First().Kind.ToString());

      target
       .Treat<TwoDisposableStringCtorClass>()
       .AsIs()
       .UsingArguments(new MemoryStream());

      // --act
      var actual = target.Build<TwoDisposableStringCtorClass>();

      // --assert
      Assert.That(actual.String, Is.EqualTo(actual.GetType().ToString()));
    }

    [Test]
    public void UsingParametersTwiceOnSameImplementationTest()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposableValue1>()
            .AsCreated<OneDisposableCtorClass>()
            .UsingArguments(new MemoryStream());

      var expected = new MemoryStream();

      target.Treat<IDisposableValue2>()
            .AsCreated<OneDisposableCtorClass>()
            .UsingArguments(expected);

      // --act
      var instance = target.Build<IDisposableValue2>();

      // --assert
      Assert.That(instance.Disposable, Is.SameAs(expected));
    }

    [Test]
    public void SameClassForMultipleKeyAsSingleton()
    {
      const string key1 = "t1";

      var target = CreateTarget();

      target.Treat<IDisposableValue1>().As<OneDisposableCtorClass>();
      target.Treat<IDisposableValue2>(key1).As<OneDisposableCtorClass>();
      target.Treat<OneDisposableCtorClass>().AsInstance(new OneDisposableCtorClass(null));

      var dep  = target.Build<IDisposableValue1>();
      var dep1 = target.UsingKey(key1).Build<IDisposableValue2>();

      Assert.AreSame(dep, dep1);
    }

    [Test(Description = "Registration of some entity separated in several parts should work")]
    public void SeparatedRegistration()
    {
      var target = CreateTarget();

      target
       .Treat<IDisposableValue1>()
       .AsCreated<OneDisposableCtorClass>();

      target
       .Treat<IDisposableValue2>()
       .AsCreated<OneDisposableCtorClass>();

      target
       .Treat<OneDisposableCtorClass>()
       .UsingArguments(new MemoryStream());

      target
       .Treat<OneDisposableCtorClass>()
       .AsSingleton();

      var actual1 = target.Build<IDisposableValue1>();
      var actual2 = target.Build<IDisposableValue2>();

      // --assert
      actual1.Should().BeOfType<OneDisposableCtorClass>();
      actual1.Should().BeSameAs(actual2);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnit(IsConstructor.Instance)
              .UseBuildAction(
                 new TryInOrder
                 {
                   new GetConstructorByInjectPointId(), // constructor marked with [Inject] attribute has more priority
                   GetConstructorWithMaxParametersCount.Instance       // constructor with largest number of parameters has less priority
                 },
                 BuildStage.Create),
             new IfLastUnit(IsParameterInfo.Instance)
              .UseBuildAction(
                 new TryInOrder
                 {
                   BuildArgumentByParameterInjectPointId.Instance, 
                   BuildArgumentByParameterType.Instance
                 }, BuildStage.Create),
             new IfLastUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfLastUnit(IsPropertyInfo.Instance)
              .UseBuildAction(
                 new TryInOrder
                 {
                   new BuildArgumentByPropertyType()
                 }, BuildStage.Create)
           }
         };

    private interface IEmptyInterface1 { }

    private interface IEmptyInterface2 { }

    private class EmptyCtorClass : IEmptyInterface1, IEmptyInterface2
    {
      private static   int _counter = 1;
      private readonly int _id      = _counter++;

      public override string ToString()
      {
        return _id.ToString();
      }
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

      public OneDisposableCtorClass(IDisposable disposable)
      {
        _disposable = disposable;
      }

      public IDisposable Disposable
      {
        get { return _disposable; }
      }
    }

    private class OneStringCtorClass : IDisposableValue1, IDisposableValue2
    {
      private readonly string _text;

      public OneStringCtorClass(string text)
      {
        _text = text;
      }

      public string Text
      {
        get { return _text; }
      }

      IDisposable IDisposableValue1.Disposable
      {
        get { throw new NotSupportedException(); }
      }

      IDisposable IDisposableValue2.Disposable
      {
        get { throw new NotSupportedException(); }
      }
    }

    private class TwoDisposableStringCtorClass : OneDisposableCtorClass
    {
      public readonly string String;

      public TwoDisposableStringCtorClass(IDisposable disposable, string @string) : base(disposable)
      {
        String = @string;
      }
    }

    private class Disposable : IDisposable
    {
      public void Dispose() { }
    }
  }
}
