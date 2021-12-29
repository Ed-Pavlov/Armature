using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning;

public class TreatingTunerTest
{
  [Test]
  public void As([Values(null, "tag")] string tag)
  {
    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.As(typeof(Stream), tag);

    // --assert
    actual.BuildActions.Should().BeEquivalentTo(Util.CreateBag(BuildStage.Create, new RedirectType(typeof(Stream), tag)));
  }
  [Test]
  public void AsT([Values(null, "tag")] string tag)
  {
    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.As<Stream>(tag);

    // --assert
    actual.BuildActions.Should().BeEquivalentTo(Util.CreateBag(BuildStage.Create, new RedirectType(typeof(Stream), tag)));
  }

  [Test]
  public void AsCreated([Values(null, "tag")] string tag)
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, new RedirectType(typeof(MemoryStream), tag));
    var expectedChildNode = new IfFirstUnit(new UnitPattern(typeof(MemoryStream), tag))
     .UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsCreated(typeof(MemoryStream), tag);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
    actual.Children.Single().Should().BeEquivalentTo(expectedChildNode);
  }

  [Test]
  public void AsCreatedT([Values(null, "tag")] string tag)
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, new RedirectType(typeof(MemoryStream), tag));
    var expectedChildNode = new IfFirstUnit(new UnitPattern(typeof(MemoryStream), tag))
     .UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsCreated<MemoryStream>(tag);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
    actual.Children.Single().Should().BeEquivalentTo(expectedChildNode);
  }

  [Test]
  public void AsInstance([Values(null, "tag")] string tag)
  {
    var expectedInstance = new MemoryStream();
    var expectedBuildActions = Util.CreateBag(BuildStage.Cache, new Instance<IDisposable>(expectedInstance));

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsInstance(expectedInstance);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }

  [Test]
  public void AsIs([Values(null, "tag")] string tag)
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, Default.CreationBuildAction);

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<MemoryStream>(actual);

    // --act
    target.AsIs();

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }

  [Test]
  public void AsCreatedWith([Values(null, "tag")] string tag)
  {
    // ReSharper disable once ConvertToLocalFunction
    Func<IBuildSession, IDisposable> factoryMethod = _ => new MemoryStream();
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, new CreateWithFactoryMethod<IDisposable>(factoryMethod));

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsCreatedWith(factoryMethod);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }
}