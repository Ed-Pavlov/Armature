using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensibility.MaybePropagation.Implementation;
using Tests.Extensibility.MaybePropagation.TestData;

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
       .AsCreated<Reader>()
       .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>());

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
       .AsCreated<Reader>()
       .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>());

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
       .AsCreated<Reader>()
       .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>());

      Action actual = () => builder.Build<Maybe<IReader>>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [Test]
    public void should_build_maybe_use_key_for_dependency()
    {
      var builder = CreateTarget();

      const string key = "key";
      builder.Treat<Maybe<Section>>(key).AsInstance(new Section().ToMaybe());

      builder
       .Treat<Maybe<IReader>>()
       .TreatMaybeValue()
       .AsCreated<Reader>()
       .BuildingWhich(_ => _.Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>(key));

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    [Test]
    public void should_build_maybe_use_inject_point_id_as_key_for_dependency()
    {
      var builder = CreateTarget();

      const string key = Reader1.InjectPointId;
      builder.Treat<Maybe<Section>>(key).AsInstance(new Section().ToMaybe());

      builder
       .Treat<Maybe<IReader>>()
       .TreatMaybeValue()
       .AsCreated<Reader1>()
       .BuildingWhich(
          _ => _
              .Treat<Section>(SpecialKey.Any)
              .AsMaybeValueOf()
              .As<Maybe<Section>>(SpecialKey.Propagate))
       .UsingParameters(ForParameter.OfType<Section>().UseInjectPointIdAsKey());

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    private static Builder CreateTarget()
      => new(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             // inject into constructor
             new IfLastUnitMatches(ConstructorPattern.Instance)
              .UseBuildAction(BuildStage.Create, GetLongestConstructor.Instance),
             new IfLastUnitMatches(MethodArgumentPattern.Instance)
              .UseBuildAction(BuildStage.Create, BuildArgumentForMethodParameter.Instance)
           }
         };
  }
}
