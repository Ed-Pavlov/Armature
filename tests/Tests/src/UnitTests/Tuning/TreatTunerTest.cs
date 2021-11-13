using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning;

public class TreatTunerTest
{
  [Test]
  public void As([Values(null, "key")] string key)
  {
    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.As(typeof(Stream), key);

    // --assert
    actual.BuildActions.Should().BeEquivalentTo(Util.CreateBag(BuildStage.Create, new RedirectType(typeof(Stream), key)));
  }
  [Test]
  public void AsT([Values(null, "key")] string key)
  {
    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.As<Stream>(key);

    // --assert
    actual.BuildActions.Should().BeEquivalentTo(Util.CreateBag(BuildStage.Create, new RedirectType(typeof(Stream), key)));
  }

  [Test]
  public void AsCreated([Values(null, "key")] string key)
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, new RedirectType(typeof(MemoryStream), key));
    var expectedChildNode = new IfFirstUnit(new UnitPattern(typeof(MemoryStream), key))
     .UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsCreated(typeof(MemoryStream), key);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
    actual.Children.Single().Should().BeEquivalentTo(expectedChildNode);
  }

  [Test]
  public void AsCreatedT([Values(null, "key")] string key)
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, new RedirectType(typeof(MemoryStream), key));
    var expectedChildNode = new IfFirstUnit(new UnitPattern(typeof(MemoryStream), key))
     .UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsCreated<MemoryStream>(key);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
    actual.Children.Single().Should().BeEquivalentTo(expectedChildNode);
  }

  [Test]
  public void AsInstance([Values(null, "key")] string key)
  {
    var expectedInstance = new MemoryStream();
    var expectedBuildActions = Util.CreateBag(BuildStage.Cache, new Instance<IDisposable>(expectedInstance));

    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsInstance(expectedInstance);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }

  [Test]
  public void AsIs([Values(null, "key")] string key)
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, Default.CreationBuildAction);

    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<MemoryStream>(actual);

    // --act
    target.AsIs();

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }

  [Test]
  public void AsCreatedWith([Values(null, "key")] string key)
  {
    // ReSharper disable once ConvertToLocalFunction
    Func<IBuildSession, IDisposable> factoryMethod = _ => new MemoryStream();
    var expectedBuildActions = Util.CreateBag(BuildStage.Create, new CreateWithFactoryMethod<IDisposable>(factoryMethod));

    // --arrange
    var actual = new Util.TestPatternTreeNode();
    var target = new TreatingTuner<IDisposable>(actual);

    // --act
    target.AsCreatedWith(factoryMethod);

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }
}