using System;
using System.Linq;
using Armature;
using Armature.Common;
using Armature.Core;
using Armature.Framework;
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
      var target = new WeakUnitSequenceMatcher(Match.Type<IDisposableValue1>(null), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      var next = new WeakUnitSequenceMatcher(Match.Type<IDisposable>(null), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
      target.AddOrGetUnitMatcher(next);
      next.AddBuildAction(BuildStage.Cache, expected, 0);

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
      var buildStep1 = new LeafUnitSequenceMatcher(Match.Type<int>(null), 0);
      buildStep1.AddBuildAction(BuildStage.Cache, CreateByReflectionBuildAction.Instance, 0);
      var singletonAction = new SingletonBuildAction();
      var buildStep2 = new AnyUnitSequenceMatcher();
      buildStep2.AddBuildAction(BuildStage.Cache, singletonAction, 0);

      var target = new WeakUnitSequenceMatcher(Match.Type<string>(null), UnitSequenceMatchingWeight.WeakMatchingTypeUnit);
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