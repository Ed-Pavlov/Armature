using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
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
      var singletonAction = new SingletonBuildAction();

      // --arrange
      var unitIdMatcher = Match.Type<string>(null);
      var matchString   = new IfLastUnitIs(unitIdMatcher).UseBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var matchAny      = new FindFirstUnit(unitIdMatcher).UseBuildAction(BuildStage.Cache, singletonAction);

      var target = new BuildPlansCollection();
      target.Children.Add(matchString);
      target.Children.Add(matchAny);

      // --act
      var actual = target.GatherBuildActions(new[] {Unit.OfType<string>()}.AsArrayTail());

      // --assert
      actual[BuildStage.Cache]
       .Should()
       .HaveCount(2)
       .And
       .Subject.Select(_ => _.Entity)
       .Should()
       .Equal(CreateByReflectionBuildAction.Instance, singletonAction);
    }
  }
}
