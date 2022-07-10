using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests
{
  public class BuildChainPatternTreeTest
  {
    [Test]
    public void should_return_all_merged_actions()
    {
      var singletonAction = new Singleton();

      // --arrange
      var unitIdMatcher = Match.Type<string>(null);
      var matchString   = new IfFirstUnit(unitIdMatcher).UseBuildAction(new CreateByReflection(), BuildStage.Cache);
      var matchAny      = new SkipTillUnit(unitIdMatcher).UseBuildAction(singletonAction, BuildStage.Cache);

      var target = new BuildChainPatternTree();
      target.Children.Add(matchString);
      target.Children.Add(matchAny);

      // --act
      var actual = target.GatherBuildActions(Unit.IsType<string>().ToBuildChain(), out var actionBag);

      // --assert
      actual.Should().BeTrue();
      actionBag![BuildStage.Cache]
       .Should()
       .HaveCount(2)
       .And
       .Subject.Select(_ => _.Entity)
       .Should()
       .Equal(Static.Of<CreateByReflection>(), singletonAction);
    }
  }
}