using System;
using System.Linq;
using Armature;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using Armature.Framework.BuildActions.Creation;
using Armature.Framework.UnitSequenceMatcher;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;
using Tests.Functional;

namespace Tests.UnitTests
{
  public class WeakBuildSequenceBuildStepTest
  {
    [Test]
    public void should_match_build_sequence_weak()
    {
      var expected = new SingletonBuildAction();

      // --arrange
      var target = new WildcardUnitSequenceMatcher(Match.Type<IDisposableValue1>(null), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      var next = new WildcardUnitSequenceMatcher(Match.Type<IDisposable>(null), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      target.AddOrGetUnitMatcher(next);
      next.AddBuildAction(BuildStage.Cache, expected);

      // --act
      var actual = target
        .GetBuildActions(new[] {Unit.OfType<IDisposableValue1>(), Unit.OfType<OneDisposableCtorClass>(), Unit.OfType<IDisposable>()}.GetTail(0), 0)
        .GetTopmostAction(BuildStage.Cache);

      // --assert
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void should_return_children_merged_actions()
    {
      // --arrange
      var buildStep1 = new LastUnitSequenceMatcher(Match.Type<int>(null), 0);
      buildStep1.AddBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance);
      var singletonAction = new SingletonBuildAction();
      var buildStep2 = new AnyUnitSequenceMatcher();
      buildStep2.AddBuildAction(BuildStage.Cache, singletonAction);

      var target = new WildcardUnitSequenceMatcher(Match.Type<string>(null), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      target.AddOrGetUnitMatcher(buildStep1);
      target.AddOrGetUnitMatcher(buildStep2);

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