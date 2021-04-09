using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
using Armature.Core.BuildActions.Property;
using Armature.Core.UnitMatchers;
using Armature.Core.UnitMatchers.Parameters;
using Armature.Core.UnitMatchers.Properties;
using Armature.Core.UnitSequenceMatcher;
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
       .AsCreatedWith(assembler => assembler.BuildSequence.First().Id.ToString());

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
      var target = CreateTarget();

      target.Treat<IDisposableValue1>()
            .AsCreated<OneDisposableCtorClass>()
            .UsingParameters(new MemoryStream());

      var expected = new MemoryStream();

      target.Treat<IDisposableValue2>()
            .AsCreated<OneDisposableCtorClass>()
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

      var target = CreateTarget();

      target.Treat<IDisposableValue1>().As<OneDisposableCtorClass>();
      target.Treat<IDisposableValue2>(token1).As<OneDisposableCtorClass>();
      target.Treat<OneDisposableCtorClass>().AsInstance(new OneDisposableCtorClass(null));

      var dep  = target.Build<IDisposableValue1>();
      var dep1 = target.UsingToken(token1).Build<IDisposableValue2>();

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
             new LastUnitSequenceMatcher(ParametersArrayMatcher.Instance)
              .AddBuildAction(BuildStage.Create, CreateParametersArrayBuildAction.Instance),
             new LastUnitSequenceMatcher(ParameterValueMatcher.Instance)
              .AddBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer {CreateParameterValueForInjectPointBuildAction.Instance, CreateParameterValueBuildAction.Instance}),
             new LastUnitSequenceMatcher(PropertyValueMatcher.Instance)
              .AddBuildAction(
                 BuildStage.Create,
                 new OrderedBuildActionContainer {new CreatePropertyValueBuildAction()})
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
