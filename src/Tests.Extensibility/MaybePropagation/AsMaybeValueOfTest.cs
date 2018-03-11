using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensibility.MaybePropagation.Extension;
using Tests.Extensibility.MaybePropagation.TestData;
using Tests.Functional;

namespace Tests.Extensibility.MaybePropagation
{
  public class AsMaybeValueOfTest
  {
    [Test]
    public void should_build_maybe()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Maybe<Section>>().AsInstance(new Section().ToMaybe());

      builder
        .Treat<IReader>()
        .Created<Reader>()
        .ByDefault();
      
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
      var builder = FunctionalTestHelper.CreateBuilder();

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
      var builder = FunctionalTestHelper.CreateBuilder();

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
      var builder = FunctionalTestHelper.CreateBuilder();

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
    
    [Ignore("Need to be implemented")]
    public void should_build_maybe_use_inject_point_id_as_token_for_dependency()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      const string token = Reader1.InjectPointId;
      builder.Treat<Maybe<Section>>(token).AsInstance(new Section().ToMaybe());

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader1>()
        .BuildingWhich(_ => _
                 .Treat<Section>()
//                 .UseInjectPointIdAsToken()
                 .AsMaybeValueOf()
//                 .PropagateTokenTo()
                 .Created<Maybe<Section>>());

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }
  }
}