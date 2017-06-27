using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Armature;
using Armature.Framework;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.Functional
{
  public class CaseTest
  {
    [Test]
    public void Building()
    {
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();
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
      var target = FunctionalTestHelper.CreateBuilder();
      target
        .Treat<IEmptyInterface1>()
        .As<EmptyCtorClass>();
      target
        .Treat<IEmptyInterface2>()
        .As<EmptyCtorClass>();
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
      var target = FunctionalTestHelper.CreateBuilder();
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
      var target = FunctionalTestHelper.CreateBuilder();

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

      var target = FunctionalTestHelper.CreateBuilder();

      target.Treat<IDisposableValue1>().As<OneDisposableCtorClass>();
      target.Treat<IDisposableValue2>(token1).As<OneDisposableCtorClass>();
      target.Treat<OneDisposableCtorClass>().AsInstance(new OneDisposableCtorClass(null));

      var dep = target.Build<IDisposableValue1>();
      var dep1 = target.WithToken(token1).Build<IDisposableValue2>();

      Assert.AreSame(dep, dep1);
    }

    [Test(Description = "Registration of some entity separated in several parts should work")]
    public void SeparatedRegistration()
    {
      var target = FunctionalTestHelper.CreateBuilder();
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

    [Test]
    public void RegisterOneInterfaceAsManyImplementations()
    {
      // --arrange
      const string oneDisposableCtorCalssToken = "disp";
      var expected1 = new OneDisposableCtorClass(null);
      var expected2 = new OneStringCtorClass(null);

      var container = FunctionalTestHelper.CreateBuilder();
      container
        .Treat<OneDisposableCtorClass>(oneDisposableCtorCalssToken)
        .AsInstance(expected1);
      container.Treat<OneStringCtorClass>()
        .AsInstance(expected2);

      // --act
      var buildStep = new UnitSequenceWeakMatchingBuildStep(Match.Type<List<IDisposableValue1>>(null));
      buildStep.AddBuildAction(
        BuildStage.Redirect,
        new RedirectManyTypesBuildAction<IDisposableValue1>(
            Unit.OfType<OneDisposableCtorClass>(oneDisposableCtorCalssToken),
            Unit.OfType<OneStringCtorClass>()));

      container.AddBuildStep(buildStep);
        
      var actual = container.Build<List<IDisposableValue1>>();

      // --assert
      actual.Should().ContainInOrder(expected1, expected2);
    }
   
    public CaseTest()
    {
      Debug.Listeners.Add(new ConsoleTraceListener());
    }
  }
}