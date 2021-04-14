using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.Common;
using Armature.Core.UnitSequenceMatcher;
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
      var target = new SkipToUnit(Match.Type<IDisposable>(null))
                  .AddItem(new SkipToUnit(Match.Type<MemoryStream>(null)))
                  .AddBuildAction(BuildStage.Cache, expected);

      // --act
      var actual = target
                  .GetBuildActions(new[] {Unit.OfType<IDisposable>(), Unit.OfType<string>(), Unit.OfType<MemoryStream>()}.GetTail(0), 0)
                  .GetTopmostAction(BuildStage.Cache);

      // --assert
      actual.Should().BeSameAs(expected);
    }

    [Test]
    public void should_return_children_merged_actions()
    {
      // --arrange
      var buildStep1 = new IfLastUnitIs(Match.Type<int>(null));
      buildStep1.AddBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var singletonAction = new SingletonBuildAction();
      var buildStep2      = new SkipToLastUnit();
      buildStep2.AddBuildAction(BuildStage.Cache, singletonAction);

      var target = new SkipToUnit(Match.Type<string>(null));
      target.AddItem(buildStep1);
      target.AddItem(buildStep2);

      // --act
      var actual = target.GetBuildActions(new[] {Unit.OfType<string>(), Unit.OfType<int>()}.GetTail(0), 0);

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
