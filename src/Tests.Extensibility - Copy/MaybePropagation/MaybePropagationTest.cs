using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Extensibility.MaybePropagation.Extension;
using Tests.Functional;

namespace Tests.Extensibility.MaybePropagation
{
  public class MaybePropagationTest
  {
    [Test]
    public void treat_maybe_of_should_inject_maybe_with_value_if_unit_is_built()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Section>().AsInstance(new Section());
      builder.Treat<Model>().AsIs();

      builder
        .TreatMaybe()
        .Of<IReader>()
        .As<Reader>();

      var actual = builder.Build<Model>();

      // --assert
      actual.Reader.HasValue.Should().BeTrue();
    }

    [Test]
    public void treat_maybe_of_should_fail_if_unit_is_not_build()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Model>().AsIs();

      // --act
      builder
        .TreatMaybe()
        .Of<IReader>()
        .As<Reader>();

      Action actual = () => builder.Build<Model>();

      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }
    
    [Test]
    public void should_propagate_maybe_value_if_maybe_value_underlying()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Maybe<Section>>().AsInstance(new Section().ToMaybe());
      builder.Treat<Model>().AsIs();

      builder
        .TreatMaybe()
        .Of<IReader>()
        .As<Reader>()
        .UsingParameters(For.Parameter<Section>().UseMaybePropagation());

      var actual = builder.Build<Model>();

      // --assert
      actual.Reader.HasValue.Should().BeTrue();
      actual.Reader.Value.Section.Should().NotBeNull();
    }
    
    [Test]
    public void should_propagate_maybe_value_if_maybe_value_with_token_underlying()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      const string token = "token";
      builder.Treat<Maybe<Section>>(token).AsInstance(new Section().ToMaybe());
      builder.Treat<Model>().AsIs();

      builder
        .TreatMaybe()
        .Of<IReader>()
        .As<Reader>()
        .UsingParameters(
          For.Parameter<Section>().UseMaybePropagation(token));

      var actual = builder.Build<Model>();

      // --assert
      actual.Reader.HasValue.Should().BeTrue();
      actual.Reader.Value.Section.Should().NotBeNull();
    }
    
    [Test]
    public void should_propagate_maybe_nothing_if_maybe_nothing_underlying()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Maybe<Section>>().AsInstance(Maybe<Section>.Nothing);
      builder.Treat<Model>().AsIs();

      builder
        .TreatMaybe()
        .Of<IReader>()
        .As<Reader>()
        .UsingParameters(For.Parameter<Section>().UseMaybePropagation());

      var actual = builder.Build<Model>();

      // --assert
      actual.Reader.HasValue.Should().BeFalse();
    }
    
    [Test]
    public void should_fail_if_none_underlying()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder.Treat<Model>().AsIs();

      builder
        .TreatMaybe()
        .Of<IReader>()
        .As<Reader>()
        .UsingParameters(For.Parameter<Section>().UseMaybePropagation());

      Action actual = () => builder.Build<Model>();

      // --assert
      actual.ShouldThrowExactly<ArmatureException>();
    }
  }
}