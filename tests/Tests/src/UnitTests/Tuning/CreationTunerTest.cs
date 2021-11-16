using System;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning;

public class CreationTunerTest
{
  [Test]
  public void should_add_default_creation_strategy([Values(null, "key")] object? key)
  {
    var expectedType = typeof(IDisposable);
    var expected     = new IfFirstUnitBuildChain(new UnitPattern(expectedType, key)).UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new BuildChainPatternTree();
    var target = new CreationTuner(actual, expectedType, key);

    // --act
    target.CreatedByDefault();

    // --assert
    actual.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void should_add_reflection_creation_strategy([Values(null, "key")] object? key)
  {
    var expectedType = typeof(IDisposable);
    var expected     = new IfFirstUnitBuildChain(new UnitPattern(expectedType, key)).UseBuildAction(new CreateByReflection(), BuildStage.Create);

    // --arrange
    var actual = new BuildChainPatternTree();
    var target = new CreationTuner(actual, expectedType, key);

    // --act
    target.CreatedByReflection();

    // --assert
    actual.Children.Single().Should().BeEquivalentTo(expected);
  }
}