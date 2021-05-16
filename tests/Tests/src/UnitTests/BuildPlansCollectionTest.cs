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
      var matchString   = new IfFirstUnit(unitIdMatcher).UseBuildAction(new CreateByReflection(), BuildStage.Cache);
      var matchAny      = new SkipTillUnit(unitIdMatcher).UseBuildAction(singletonAction, BuildStage.Cache);

      var target = new PatternTree();
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
       .Equal(Static<CreateByReflection>.Instance, singletonAction);
    }
  }
}
