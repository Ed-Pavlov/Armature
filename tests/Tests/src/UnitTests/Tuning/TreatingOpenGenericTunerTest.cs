using System.Collections.Generic;
using System.Linq;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.Tuning;

public class TreatingOpenGenericTunerTest
{
  [Test]
  public void As([Values(null, "tag")] object? tag)
  {
    var expectedType = typeof(List<>);
    var expected     = new RedirectOpenGenericType(expectedType, tag);

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingOpenGenericTuner(actual);

    // --act
    target.As(expectedType, tag);

    // --assert
    actual.BuildActions.Should()
          .HaveCount(1)
          .And.ContainKey(BuildStage.Create)
          .And.Subject.Values.Single()
          .Should()
          .HaveCount(1)
          .And.Contain(expected);
  }

  [Test]
  public void AsCreated([Values(null, "tag")] object? tag)
  {
    var expectedType   = typeof(List<>);
    var expectedAction = new RedirectOpenGenericType(expectedType, tag);
    var expectedChild = new IfFirstUnit(new IsGenericOfDefinition(expectedType, tag))
       .UseBuildAction(Default.CreationBuildAction, BuildStage.Create);

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingOpenGenericTuner(actual);

    // --act
    target.AsCreated(expectedType, tag);

    // --assert
    actual.BuildActions.Should()
          .HaveCount(1)
          .And.ContainKey(BuildStage.Create)
          .And.Subject.Values.Single()
          .Should()
          .HaveCount(1)
          .And.Contain(expectedAction);

    actual.Children.Single().Should().BeEquivalentTo(expectedChild);
  }

  [Test]
  public void AsIs()
  {
    var expectedAction = Default.CreationBuildAction;

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new TreatingOpenGenericTuner(actual);

    // --act
    target.AsIs();

    // --assert
    actual.BuildActions.Should()
          .HaveCount(1)
          .And.ContainKey(BuildStage.Create)
          .And.Subject.Values.Single()
          .Should()
          .HaveCount(1)
          .And.Contain(expectedAction);
  }
}