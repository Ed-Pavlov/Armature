using System.Linq;
using Armature.Core;
using Armature.Framework;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests
{
  public class BuildPlansCollectionTest
  {
    [Test]
    public void should_return_all_merged_actions()
    {
      // --arrange
      var buildStep1 = new AnyUnitBuildStep().AddBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var singletonAction = new SingletonBuildAction();
      var buildStep2 = new AnyUnitBuildStep().AddBuildAction(BuildStage.Cache, singletonAction);

      var target = new BuildPlansCollection();
      target.AddBuildStep(buildStep1);
      target.AddBuildStep(buildStep2);

      // --act
      var actual = target.GetBuildActions(new[] {Unit.OfType<string>()});

      // --assert
      actual[BuildStage.Cache]
        .Should()
        .HaveCount(2)
        .And
        .Subject.Select(_ => _.BuildAction)
        .Should()
        .BeEquivalentTo(singletonAction, CreateByReflectionBuildAction.Instance);
    }
  }
}