using System;
using Armature;
using Armature.Core;
using Armature.Framework;
using Armature.Logging;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensibility.MaybePropagation.Implementation;
using Tests.Extensibility.MaybePropagation.TestData;
using Tests.Functional;

namespace Tests.Extensibility.MaybePropagation
{
  public class AsMaybeValueOfTest
  {
    [Test]
    public void should_build_maybe()
    {
      var builder = CreateTarget();

      builder.Treat<Maybe<Section>>().AsInstance(new Section().ToMaybe());

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader>()
        .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().Created<Maybe<Section>>());

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.Should().NotBeNull();
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    [Test]
    public void should_build_maybe_nothing_if_dependency_is_nothing()
    {
      var builder = CreateTarget();

      builder.Treat<Maybe<Section>>().AsInstance(Maybe<Section>.Nothing);

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader>()
        .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().Created<Maybe<Section>>());

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.Should().NotBeNull();
      actual.HasValue.Should().BeFalse();
    }

    [Test]
    public void should_not_build_maybe_if_dependency_cant_be_built()
    {
      var builder = CreateTarget();

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader>()
        .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().Created<Maybe<Section>>());

      Action actual = () => builder.Build<Maybe<IReader>>();

      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }

    [Test]
    public void should_build_maybe_use_token_for_dependency()
    {
      var builder = CreateTarget();

      const string token = "token";
      builder.Treat<Maybe<Section>>(token).AsInstance(new Section().ToMaybe());

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader>()
        .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().Created<Maybe<Section>>(token));

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    [Test]
    public void should_build_maybe_use_inject_point_id_as_token_for_dependency()
    {
      var builder = CreateTarget();

      const string token = Reader1.InjectPointId;
      builder.Treat<Maybe<Section>>(token).AsInstance(new Section().ToMaybe());

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader1>()
        .BuildingWhich(
          _ => _
            .Treat<Section>(Token.Any)
            .AsMaybeValueOf()
            .Created<Maybe<Section>>(Token.Propagate))
        .UsingParameters(For.Parameter<Section>().UseInjectPointIdAsToken());

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    private static Builder CreateTarget() =>
      FunctionalTestHelper.CreateBuilder(null, BuildStage.Cache, Extension.GetMaybeValueStage, BuildStage.Initialize, BuildStage.Create);
  }
}