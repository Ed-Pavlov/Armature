using System.Collections.Generic;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning;

public class OpenGenericCreationTunerTest
{
  [Test]
  public void should_add_default_creation_strategy([Values(null, "tag")] object? tag)
  {
    var expectedType = typeof(IList<>);
    var expected     = new IfFirstUnit(new IsGenericOfDefinition(expectedType, tag)).UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new BuildChainPatternTree();
    var target = new OpenGenericCreationTuner(actual, expectedType, tag);

    // --act
    target.CreatedByDefault();

    // --assert
    actual.Children.Single().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void should_add_reflection_creation_strategy([Values(null, "tag")] object? tag)
  {
    var expectedType = typeof(IList<>);
    var expected     = new IfFirstUnit(new IsGenericOfDefinition(expectedType, tag)).UseBuildAction(new CreateByReflection(), BuildStage.Create);

    // --arrange
    var actual = new BuildChainPatternTree();
    var target = new OpenGenericCreationTuner(actual, expectedType, tag);

    // --act
    target.CreatedByReflection();

    // --assert
    actual.Children.Single().Should().BeEquivalentTo(expected);
  }
}