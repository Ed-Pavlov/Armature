using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.BuildActions;
using Armature.BuildActions.Constructor;
using Armature.BuildActions.Method;
using Armature.BuildActions.Property;
using Armature.Core;
using Armature.Core.Sdk;
using Armature.UnitPatterns;
using Armature.UnitPatterns.Method;
using Armature.UnitPatterns.Property;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using Tests.Util;

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
       .AsCreatedWith(buildSession => buildSession.Stack.AsEnumerable().Last().Kind!.ToString());

      target
       .Treat<DisposableAndStringCtorClass>()
       .AsIs()
       .UsingArguments(new MemoryStream());

      // --act
      var actual = target.Build<DisposableAndStringCtorClass>()!;

      // --assert
      actual.Should().NotBeNull();
      actual.String.Should().Be(actual.GetType().ToString());
    }

    [Test]
    public void UsingParametersTwiceOnSameImplementationTest()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposableValue1>()
            .AsCreated<DisposableCtorClass>()
            .UsingArguments(new MemoryStream());

      var expected = new MemoryStream();

      target.Treat<IDisposableValue2>()
            .AsCreated<DisposableCtorClass>()
            .UsingArguments(expected);

      // --act
      var instance = target.Build<IDisposableValue2>()!;

      // --assert
      instance.Should().NotBeNull();
      instance.Value.Should().BeSameAs(expected);
    }


    [Test(Description = "Registration of some entity separated in several parts should work")]
    public void SeparatedRegistration()
    {
      var target = CreateTarget();

      target
       .Treat<IDisposableValue1>()
       .AsCreated<DisposableCtorClass>();

      target
       .Treat<IDisposableValue2>()
       .AsCreated<DisposableCtorClass>();

      target
       .Treat<DisposableCtorClass>()
       .UsingArguments(new MemoryStream());

      target
       .Treat<DisposableCtorClass>()
       .AsSingleton();

      var actual1 = target.Build<IDisposableValue1>();
      var actual2 = target.Build<IDisposableValue2>();

      // --assert
      actual1.Should().BeOfType<DisposableCtorClass>();
      actual1.Should().BeSameAs(actual2);
    }
    [Test]
    public void SameClassForMultipleTagAsSingleton()
    {
      const string tag1 = "t1";

      var target = CreateTarget();

      target.Treat<IDisposableValue1>().As<DisposableCtorClass>();
      target.Treat<IDisposableValue2>(tag1).As<DisposableCtorClass>();
      target.Treat<DisposableCtorClass>().AsInstance(new DisposableCtorClass(null));

      var dep  = target.Build<IDisposableValue1>();
      var dep1 = target.UsingTag(tag1).Build<IDisposableValue2>();

      Assert.AreSame(dep, dep1);
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
             // inject into constructor
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(
                 new TryInOrder
                 {
                   new GetConstructorByInjectPoint(),              // constructor marked with [Inject] attribute has more priority
                   Static.Of<GetConstructorWithMaxParametersCount>() // constructor with largest number of parameters has less priority
                 },
                 BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(
                 new TryInOrder
                 {
                   Static.Of<BuildArgumentByParameterTypeAndTag>(),
                   Static.Of<BuildArgumentByParameterType>()
                 }, BuildStage.Create),
             new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsPropertyInfo())
              .UseBuildAction(
                 new TryInOrder
                 {
                   new BuildArgumentByPropertyType()
                 }, BuildStage.Create)
         };

    private interface IEmptyInterface1 { }

    private interface IEmptyInterface2 { }

    [UsedImplicitly]
    private class EmptyCtorClass : IEmptyInterface1, IEmptyInterface2
    {
      private static   int _counter = 1;
      private readonly int _id      = _counter++;

      public override string ToString() => _id.ToString();
    }

    private interface IDisposableValue1
    {
      IDisposable? Value { get; }
    }

    private interface IDisposableValue2
    {
      IDisposable? Value { get; }
    }

    private class DisposableCtorClass : IDisposableValue1, IDisposableValue2
    {
      public DisposableCtorClass(IDisposable? value) => Value = value;

      public IDisposable? Value { get; }
    }

    [UsedImplicitly]
    private class DisposableAndStringCtorClass : DisposableCtorClass
    {
      public readonly string String;

      public DisposableAndStringCtorClass(IDisposable? value, string @string) : base(value) => String = @string;
    }
  }
}