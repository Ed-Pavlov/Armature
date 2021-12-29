using System;
using System.Collections.Generic;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning;

public class RootTunerTest
{
  [Test]
  public void treat([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new UnitPattern(typeof(IDisposable), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Treat(typeof(IDisposable), tag);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_generic([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new UnitPattern(typeof(IDisposable), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Treat<IDisposable>(tag);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void building([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new UnitPattern(typeof(IDisposable), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Building(typeof(IDisposable), tag);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void building_generic([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new UnitPattern(typeof(IDisposable), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Building<IDisposable>(tag);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void amend_weight([Values(null, "tag")] string tag)
  {
    const int weight = 3875;

    var expected = new SkipWhileUnit(new UnitPattern(typeof(IDisposable), tag), weight);

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Building<IDisposable>(tag)
          .AmendWeight(weight);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_all([Values(null, "tag")] string tag)
  {
    var expected = new SkipAllUnits();

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.TreatAll();

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_inheritors([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new IsInheritorOf(typeof(IDisposable), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    target.TreatInheritorsOf(typeof(IDisposable), tag);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_inheritors_generic([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new IsInheritorOf(typeof(IDisposable), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    // --assert
    target.TreatInheritorsOf<IDisposable>(tag);
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_open_generic([Values(null, "tag")] string tag)
  {
    var expected = new SkipWhileUnit(new IsGenericOfDefinition(typeof(List<>), tag));

    // --arrange
    var tree   = new BuildChainPatternTree();
    var target = new RootTuner(tree);

    // --act
    var tuner = target.TreatOpenGeneric(typeof(List<>), tag);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
    tuner.Should().BeOfType<TreatingOpenGenericTuner>();
  }
}