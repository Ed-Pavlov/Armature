using System.Linq;
using Armature;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
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
      var buildStep1 = new LeafUnitSequenceMatcher(Match.Type<string>(null), 0);
      buildStep1.AddBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var singletonAction = new SingletonBuildAction();
      var buildStep2 = new AnyUnitSequenceMatcher();
      buildStep2.AddBuildAction(BuildStage.Cache, singletonAction);

      var target = new BuildPlansCollection();
      target.AddUnitMatcher(buildStep1);
      target.AddUnitMatcher(buildStep2);

      // --act
      var actual = target.GetBuildActions(new[] {Unit.OfType<string>()});

      // --assert
      actual[BuildStage.Cache]
        .Should()
        .HaveCount(2)
        .And
        .Subject.Select(_ => _.Entity)
        .Should()
        .BeEquivalentTo(singletonAction, CreateByReflectionBuildAction.Instance);
    }
  }
}