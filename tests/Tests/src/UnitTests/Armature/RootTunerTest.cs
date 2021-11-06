using System;
using System.Collections.Generic;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Armature;

public class RootTunerTest
{
  [Test]
  public void treat([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new Pattern(typeof(IDisposable), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Treat(typeof(IDisposable), key);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void generic_treat([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new Pattern(typeof(IDisposable), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Treat<IDisposable>(key);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void building([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new Pattern(typeof(IDisposable), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Building(typeof(IDisposable), key);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void generic_building([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new Pattern(typeof(IDisposable), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Building<IDisposable>(key);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void add_weight([Values(null, "key")] string key)
  {
    const int weight = 3875;

    var expected = new SkipWhileUnit(new Pattern(typeof(IDisposable), key), weight);

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.Building<IDisposable>(key)
          .AddWeight(weight);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_all([Values(null, "key")] string key)
  {
    var expected = new SkipAllUnits();

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.TreatAll();

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_inheritors([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new IsSubtypeOf(typeof(IDisposable), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    target.TreatInheritorsOf(typeof(IDisposable), key);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void generic_treat_inheritors([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new IsSubtypeOf(typeof(IDisposable), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    // --assert
    target.TreatInheritorsOf<IDisposable>(key);
    tree.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void treat_open_generic([Values(null, "key")] string key)
  {
    var expected = new SkipWhileUnit(new IsOpenGenericType(typeof(List<>), key));

    // --arrange
    var tree   = new PatternTree();
    var target = new RootTuner(tree);

    // --act
    var tuner = target.TreatOpenGeneric(typeof(List<>), key);

    // --assert
    tree.Children.Single().Should().BeEquivalentTo(expected);
    tuner.Should().BeOfType<TreatingOpenGenericTuner>();
  }
}