using System.Linq;
using Armature;
using Armature.Core;
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
      var singletonAction = new Singleton();

      // --arrange
      var unitIdMatcher = Match.Type<string>(null);
      var matchString   = new IfLastUnitMatches(unitIdMatcher).UseBuildAction(BuildStage.Cache, CreateByReflection.Instance);
      var matchAny      = new FindUnitMatches(unitIdMatcher).UseBuildAction(BuildStage.Cache, singletonAction);

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
       .Equal(CreateByReflection.Instance, singletonAction);
    }
  }
}
