using System;
using System.Collections.Generic;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning.InjectionPoints;

public class ConstructorTest
{
  [Test]
  public void should_tune_getting_constructor_without_parameters()
  {
    var expected = new IfFirstUnit(Static.Of<IsConstructor>()).UseBuildAction(new GetConstructorByParameterTypes(), BuildStage.Create);
    // --arrange
    var target = Constructor.Parameterless();
    var root = new BuildChainPatternTree();

    // --act
    target.Tune(root);

    // --assert
    root.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void should_tune_getting_constructor_with_parameters([ValueSource(nameof(constructor_parameters_source))] Type[] types)
  {
    var expected = new IfFirstUnit(Static.Of<IsConstructor>()).UseBuildAction(new GetConstructorByParameterTypes(types), BuildStage.Create);

    // --arrange
    var target = Constructor.WithParameters(types);
    var root = new BuildChainPatternTree();

    // --act
    target.Tune(root);

    // --assert
    root.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void should_tune_getting_constructor_with_parameters_1()
  {
    var expected = new IfFirstUnit(Static.Of<IsConstructor>()).UseBuildAction(new GetConstructorByParameterTypes(typeof(int)), BuildStage.Create);

    // --arrange
    var target = Constructor.WithParameters<int>();
    var root = new BuildChainPatternTree();

    // --act
    target.Tune(root);

    // --assert
    root.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void should_tune_getting_constructor_marked_with_inject_attribute([Values(null, "id")] object? id)
  {
    var expected = new IfFirstUnit(Static.Of<IsConstructor>()).UseBuildAction(new GetConstructorByInjectPointId(id), BuildStage.Create);

    // --arrange
    var target = Constructor.MarkedWithInjectAttribute(id);
    var root   = new BuildChainPatternTree();

    // --act
    target.Tune(root);

    // --assert
    root.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void should_tune_getting_constructor_with_max_parameters_count()
  {
    var expected = new IfFirstUnit(Static.Of<IsConstructor>()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create);

    // --arrange
    var target = Constructor.WithMaxParametersCount();
    var root   = new BuildChainPatternTree();

    // --act
    target.Tune(root);

    // --assert
    root.Children.Single().Should().BeEquivalentTo(expected);
  }

  public static IEnumerable<Type[]> constructor_parameters_source()
  {
    yield return new[] {typeof(int)};
    yield return new[] {typeof(string), typeof(int)};
    yield return new[] {typeof(int), typeof(string)};
  }

  private class Root : BuildChainPatternBase
  {
    public Root(int expectedWeight) : base(expectedWeight) { }

    public override WeightedBuildActionBag? GatherBuildActions(BuildChain buildChain, int inputWeight) => GetOwnBuildActions(Weight);
  }
}