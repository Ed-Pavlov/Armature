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
using WeightOf = BeatyBit.Armature.Core.WeightOf;

namespace Tests.Extensibility.MaybePropagation
{
  public class TreatMaybeValueTest
  {
      [Test]
    public void new_test()
    {
      var builder = CreateTarget();

      builder.AddNode(
        new IfFirstUnit(new IsGenericOfDefinition(typeof(Maybe<>), ServiceTag.Any), WeightOf.BuildStackPattern.IfFirstUnit)
         .UseBuildAction(new BuildMaybe(null), BuildStage.Create));

      builder.Treat<Section>().AsInstance(new Section());

      builder
       .Treat<IReader>()
       .AsCreated<Reader>();


      var actual = builder.Build<Maybe<IReader>>()!;

      // --assert
      actual.HasValue.Should().BeTrue();
      actual.Value.Should().BeOfType<Reader>();
    }

    [Test]
    public void should_build_maybe()
    {
      var builder = CreateTarget();

      builder.Treat<Section>().AsInstance(new Section());

      // Maybe<IReader> -> Create(IReader, guid)
      // IReader, guid -> Redirect(Reader, null)
      // Reader, null
      //   Maybe<IReader> -> Create

      builder
       .Treat<Maybe<IReader>>()
       .TreatMaybeValue()
       .AsCreated<Reader>();

      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
    }

    [Test]
    public void should_not_build_maybe_if_dependency_cant_be_built()
    {
      var builder = CreateTarget();

      // --act
      builder
       .Treat<Maybe<IReader>>()
       .TreatMaybeValue()
       .AsCreated<Reader>();

      Action actual = () => builder.Build<Maybe<IReader>>();

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    private static Builder CreateTarget()
      => new("test", BuildStage.Cache, BuildStage.Create)
         {
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create)
         };
  }
}