using System;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensibility.MaybePropagation.Implementation;
using Tests.Extensibility.MaybePropagation.TestData;

namespace Tests.Extensibility.MaybePropagation
{
  public class TreatMaybeValueTest
  {
    [Test]
    public void should_build_maybe()
    {
      var builder = CreateTarget();

      builder.Treat<Section>().AsInstance(new Section());

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
      => new(BuildStage.Cache, BuildStage.Create)
         {
           new SkipAllUnits
           { // inject into constructor
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfoList())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create)
           }
         };
  }
}
