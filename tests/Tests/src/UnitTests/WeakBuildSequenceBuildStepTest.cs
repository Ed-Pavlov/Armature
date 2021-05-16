using System;
using System.IO;
using System.Linq;
using Armature;
using Armature.Core;
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
      var expected = new Singleton();

      // --arrange
      var target = new SkipTillUnit(Match.Type<IDisposable>(null))
                  .GetOrAddNode(new SkipTillUnit(Match.Type<MemoryStream>(null)))
                  .UseBuildAction(expected, BuildStage.Cache);

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
      var buildStep1    = new IfFirstUnit(unitIdMatcher);
      buildStep1.UseBuildAction(Static<CreateByReflection>.Instance, BuildStage.Cache);
      var singletonAction = new Singleton();
      var buildStep2      = new SkipTillUnit(unitIdMatcher);
      buildStep2.UseBuildAction(singletonAction, BuildStage.Cache);

      var target = new SkipTillUnit(Match.Type<string>(null));
      target.GetOrAddNode(buildStep1);
      target.GetOrAddNode(buildStep2);

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
       .BeEquivalentTo(Static<CreateByReflection>.Instance, singletonAction);
    }

    [Test]
    public void hz()
    {
      Console.WriteLine(WeightOf.SkipToLastUnit);
      Console.WriteLine(WeightOf.Lowest << (sizeof(byte) * 8));
      Console.WriteLine(WeightOf.FindUnit);
      Console.WriteLine(WeightOf.FirstUnit);
    }
  }
}
