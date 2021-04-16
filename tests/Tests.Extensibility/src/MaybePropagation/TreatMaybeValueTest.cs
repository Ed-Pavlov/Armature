using System;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using Armature.Core.BuildActions.Parameter;
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
           new SkipToLastUnit
           { // inject into constructor
             new IfLastUnit(IsConstructorMatcher.Instance)
              .UseBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance),
             new IfLastUnit(IsParameterMatcher.Instance)
              .UseBuildAction(BuildStage.Create, CreateParameterValueBuildAction.Instance)
           }
         };
  }
}
