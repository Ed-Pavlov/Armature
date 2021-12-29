using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.UnitTests;

namespace Tests.Integration;

public class ForParameterTest
{
  private static readonly ParameterInfo[] ParameterList   = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters();
  private static readonly UnitId[]        BuildChain   = {new UnitId(ParameterList, SpecialTag.Argument)};
  private static readonly UnitId          NamedParameter  = new(ParameterList.Single(_ => _.Name          == Subject.ParamName), SpecialTag.Argument);
  private static readonly UnitId          TypedParameter  = new(ParameterList.Single(_ => _.ParameterType == typeof(int)), SpecialTag.Argument);
  private static readonly UnitId          IdNullParameter = new(ParameterList.Single(_ => _.ParameterType == typeof(string)), SpecialTag.Argument);
  private static readonly UnitId          IdParameter     = new(ParameterList.Single(_ => _.ParameterType == typeof(IDisposable)), SpecialTag.Argument);

  private static readonly List<IForParameter> ForParameterWithInjectAttributeCases =
      new()
      {
          new ArrangeForParameterWithInjectAttribute($"{nameof(ForParameter)}.{nameof(ForParameter.WithInjectPoint)}(null)", () => ForParameter.WithInjectPoint(null), BuildChain.Concat(new[] {IdNullParameter}).ToArray().ToArrayTail()),
          new ArrangeForParameterWithInjectAttribute($"{nameof(ForParameter)}.{nameof(ForParameter.WithInjectPoint)}({Subject.Id})", () => ForParameter.WithInjectPoint(Subject.Id), BuildChain.Concat(new[] {IdParameter}).ToArray().ToArrayTail()),
      };
  private static readonly List<IForParameter> ForParameterCases =
      new()
      {

          new ArrangeForParameter<object?>($"{nameof(ForParameter)}.{nameof(ForParameter.Named)}({Subject.ParamName})", () => ForParameter.Named(Subject.ParamName), BuildChain.Concat(new[] {NamedParameter}).ToArray().ToArrayTail()),
          // ReSharper disable once ConvertClosureToMethodGroup
          new ArrangeForParameter<int>($"{nameof(ForParameter)}.{nameof(ForParameter.OfType)}<int>()", () => ForParameter.OfType<int>(), BuildChain.Concat(new[] {TypedParameter}).ToArray().ToArrayTail()),
          new ArrangeForParameter<object?>($"{nameof(ForParameter)}.{nameof(ForParameter.OfType)}(typeof(int))", () => ForParameter.OfType(typeof(int)), BuildChain.Concat(new[] {TypedParameter}).ToArray().ToArrayTail()),
      };

  private static IEnumerable<ActAssert> CombineCommonCases<T>(ArrangeForParameter<T> arrange)
  {
    yield return new ActAssert(
        $"{arrange.Name}.{nameof(MethodArgumentTuner.UseTag)}",
        () =>
        {
          const string tag = "unitTag";

          // --arrange
          var argumentTuner = arrange.ForParameter().UseTag(tag);
          var patternTree   = new BuildChainPatternTree();
          argumentTuner.Tune(patternTree);

          // --act
          patternTree.GatherBuildActions(arrange.Chain, out var actionBag);

          // --assert
          actionBag!.Keys.Should().HaveCount(1).And.Contain(BuildStage.Create);
          actionBag.Values.Single().Single().Entity.Should().Be(new BuildArgumentByParameterType(tag));
        });

    yield return new ActAssert(
        $"{arrange.Name}.{nameof(MethodArgumentTuner.UseValue)}",
        () =>
        {
          var value = Activator.CreateInstance<T>();

          // --arrange
          var argumentTuner = arrange.ForParameter().UseValue(value);
          var patternTree   = new BuildChainPatternTree();
          argumentTuner.Tune(patternTree);

          // --act
          var result = patternTree.GatherBuildActions(arrange.Chain, out var actionBag);

          // --assert
          result.Should().BeTrue();
          actionBag!.Keys.Should().HaveCount(1).And.Contain(BuildStage.Create);
          actionBag.Values.Single().Single().Entity.Should().Be(new Instance<T>(value));
        });

    yield return new ActAssert(
        $"{arrange.Name}.{nameof(MethodArgumentTuner.UseValue)}({default})",
        () =>
        {
          // --arrange
          var argumentTuner = arrange.ForParameter().UseValue(default);
          var patternTree   = new BuildChainPatternTree();
          argumentTuner.Tune(patternTree);

          // --act
          var result = patternTree.GatherBuildActions(arrange.Chain, out var actionBag);

          // --assert
          result.Should().BeTrue();
          actionBag!.Keys.Should().HaveCount(1).And.Contain(BuildStage.Create);
          actionBag.Values.Single().Single().Entity.Should().Be(new Instance<T>(default));
        });

    yield return new ActAssert(
        $"{arrange.Name}.{nameof(MethodArgumentTuner.UseFactoryMethod)}(Func<{typeof(T).Name}>)",
        () =>
        {
          // --arrange
          var argumentTuner = arrange.ForParameter().UseFactoryMethod(() => default);
          var patternTree   = new BuildChainPatternTree();
          argumentTuner.Tune(patternTree);

          // --act
          var result = patternTree.GatherBuildActions(arrange.Chain, out var actionBag);

          // --assert
          result.Should().BeTrue();
          actionBag!.Keys.Should().HaveCount(1).And.Contain(BuildStage.Create);
          actionBag.Values.Single().Single().Entity.Should().BeOfType(typeof(CreateWithFactoryMethod<T>));
        });
  }

  private static IEnumerable<ActAssert> CombineInjectAttributeCases<T>(ArrangeForParameter<T> arrange)
  {
    yield return new ActAssert(
        $"{arrange.Name}.{nameof(MethodArgumentTuner.UseInjectPointIdAsTag)}",
        () =>
        {
          // --arrange
          var argumentTuner = arrange.ForParameter().UseInjectPointIdAsTag();
          var patternTree   = new BuildChainPatternTree();
          argumentTuner.Tune(patternTree);

          // --act
          var result = patternTree.GatherBuildActions(arrange.Chain, out var actionBag);

          // --assert
          result.Should().BeTrue();
          actionBag!.Keys.Should().HaveCount(1).And.Contain(BuildStage.Create);
          actionBag.Values.Single().Single().Entity.Should().Be(new BuildArgumentByParameterInjectPointId());
        });
  }

  [TestCaseSource(nameof(CommonCases))]
  public void test_common_parameters_with_suitable_registrations(ActAssert actAssert) => actAssert.Execute();

  [TestCaseSource(nameof(InjectAttributeCases))]
  public void test_inject_attributed_parameters_with_suitable_registrations(ActAssert actAssert) => actAssert.Execute();

  public record ActAssert(string Name, Action Do)
  {
    public void Execute() => Do();

    public override string ToString() => Name;
  }

  private interface IForParameter
  {
    IEnumerable CreateCases();
  }

  private record ArrangeForParameter<T>(string Name, Func<MethodArgumentTuner<T>> ForParameter, BuildChain Chain) : IForParameter
  {
    public virtual IEnumerable CreateCases() => CombineCommonCases(this);
  }

  private record ArrangeForParameterWithInjectAttribute(string Name, Func<MethodArgumentTuner<object?>> ForParameter, BuildChain Chain)
      : ArrangeForParameter<object?>(Name, ForParameter, Chain)
  {
    public override IEnumerable CreateCases() => CombineCommonCases(this).Concat(CombineInjectAttributeCases(this));
  }

  private static IEnumerable CommonCases()
  {
    foreach(var forParameter in ForParameterCases)
    foreach(var @case in forParameter.CreateCases())
      yield return @case;
  }

  private static IEnumerable InjectAttributeCases()
  {
    foreach(var forParameter in ForParameterWithInjectAttributeCases)
    foreach(var @case in forParameter.CreateCases())
      yield return @case;
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public const  string ParamName  = "optional";
    public const  string Id = "id";

    public static void   Foo(int i, [Inject] string str, [Inject(Id)] IDisposable obj, bool optional = true) {}
  }
}