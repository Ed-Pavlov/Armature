using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensibility.MaybePropagation.Implementation;
using Tests.Extensibility.MaybePropagation.TestData;
using Tests.Functional;

namespace Tests.Extensibility.MaybePropagation
{
  public class TreatMaybeValueTest
  {
    [Test]
    public void should_build_maybe()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Section>().AsInstance(new Section());

      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader>();
      
      var actual = builder.Build<Maybe<IReader>>();

      // --assert
      actual.HasValue.Should().BeTrue();
    }

    [Test]
    public void should_not_build_maybe_if_dependency_cant_be_built()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      // --act
      builder
        .Treat<Maybe<IReader>>()
        .TreatMaybeValue()
        .As<Reader>();

      Action actual = () => builder.Build<Maybe<IReader>>();

      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }
  }
}