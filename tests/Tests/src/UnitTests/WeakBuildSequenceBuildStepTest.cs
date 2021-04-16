using System;
using System.IO;
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
  public class WeakBuildSequenceBuildStepTest
  {
    [Test]
    public void should_match_build_sequence_weak()
    {
      var expected = new SingletonBuildAction();

      // --arrange
      var target = new FindFirstUnit(Match.Type<IDisposable>(null))
                  .AddSubQuery(new FindFirstUnit(Match.Type<MemoryStream>(null)))
                  .UseBuildAction(BuildStage.Cache, expected);

      // --act
      var actual = target
                  .GatherBuildActions(new[] {Unit.OfType<IDisposable>(), Unit.OfType<string>(), Unit.OfType<MemoryStream>()}.GetTail(0), 0)
                  .GetTopmostAction(BuildStage.Cache);

      // --assert
      actual.Should().BeSameAs(expected);
    }

    [Test]
    public void should_return_children_merged_actions()
    {
      // --arrange
      var unitIdMatcher = Match.Type<int>(null);
      var buildStep1    = new IfLastUnitIs(unitIdMatcher);
      buildStep1.UseBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var singletonAction = new SingletonBuildAction();
      var buildStep2      = new FindFirstUnit(unitIdMatcher);
      buildStep2.UseBuildAction(BuildStage.Cache, singletonAction);

      var target = new FindFirstUnit(Match.Type<string>(null));
      target.AddSubQuery(buildStep1);
      target.AddSubQuery(buildStep2);

      // --act
      var actual = target.GatherBuildActions(new[] {Unit.OfType<string>(), Unit.OfType<int>()}.GetTail(0), 0);

      // --assert
      actual.Should().NotBeNull();

      // ReSharper disable once PossibleNullReferenceException
      actual[BuildStage.Cache]
       .Should()
       .HaveCount(2)
       .And
       .Subject.Select(_ => _.Entity)
       .Should()
       .BeEquivalentTo(CreateByReflectionBuildAction.Instance, singletonAction);
    }
  }
}
