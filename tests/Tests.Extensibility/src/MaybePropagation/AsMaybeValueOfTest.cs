using System;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using BeatyBit.Armature.Sdk;
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
       .Treat<Maybe<IReader>>()       // IfFirst<Maybe<IReader>> -> BuildAction build IReader.ToMaybe
       .TreatMaybeValue()             //   IfFirst<IReader>
       .AsCreated<Reader>();          //                     -> BuildAction new Reader

      builder
       .Building<Reader>()           // IfFirst<Section>
       .Treat<Section>()             //   SkipTill<Reader>   -> Redirect<Maybe<Section>>
       .AsMaybeValueOf()   //                       -> BuildResult.Value as Maybe .Value | throw
       .As<Maybe<Section>>();

      var actual = builder.Build<Maybe<IReader>>()!;

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
       .AsCreated<Reader>();

      builder.Building<Reader>().Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>();

      var actual = builder.Build<Maybe<IReader>>()!;

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
       .AsCreated<Reader>();

      builder.Building<Reader>().Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>();

      Action actual = () => builder.Build<Maybe<IReader>>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [Test]
    public void should_build_maybe_use_tag_for_dependency()
    {
      var builder = CreateTarget();

      const string tag = "tag";
      builder.Treat<Maybe<Section>>(tag).AsInstance(new Section().ToMaybe());

      builder
       .Treat<Maybe<IReader>>()
       .TreatMaybeValue()
       .AsCreated<Reader>();

      builder.Building<Reader>().Treat<Section>().AsMaybeValueOf().As<Maybe<Section>>(tag);

      var actual = builder.Build<Maybe<IReader>>()!;

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    [Test]
    public void should_build_maybe_use_inject_point_id_as_tag_for_dependency()
    {
      var builder = CreateTarget();

      const string tag = Reader1.InjectPointId;
      builder.Treat<Maybe<Section>>(tag).AsInstance(new Section().ToMaybe());

      builder
       .Treat<Maybe<IReader>>()
       .TreatMaybeValue()
       .AsCreated<Reader1>()
       .UsingArguments(ForParameter.OfType<Section>().UseInjectPointTag());

      builder
       .Building<Reader1>()
       .Treat<Section>(ServiceTag.Any)
       .AsMaybeValueOf()
       .As<Maybe<Section>>(ServiceTag.Propagate);

      var actual = builder.Build<Maybe<IReader>>()!;

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Section.Should().NotBeNull();
    }

    private static Builder CreateTarget()
      => new("test", BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
         {
             // inject into constructor
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create)
         };
  }
}