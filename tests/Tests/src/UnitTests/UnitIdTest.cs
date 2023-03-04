using System;
using Armature.Core;
using Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests;

public class UnitIdTest
{
  [Test]
  public void kind_or_tag_should_be_not_null()
  {
    // --arrange
    var actual = () => Unit.Of(null);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Either kind or tag should be provided*");
  }

  [Test]
  public void should_be_equal([Values(null, "kind")] object? kind, [Values(null, "tag")] object? tag)
  {
    if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = Unit.Of(kind, tag);
    var unit2 = Unit.Of(kind, tag);

    // --assert
    unit1.Equals(unit2).Should().BeTrue();
    unit2.Equals(unit1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal([Values(null, "kind")] object? kind, [Values(null, "tag")] object? tag)
  {
    if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = Unit.Of(tag, kind);
    var unit2 = Unit.Of(kind, tag);

    // --assert
    unit1.Equals(unit2).Should().BeFalse();
    unit2.Equals(unit1).Should().BeFalse();
  }

  [Test]
  public void should_ignore_special_tag([Values(null, "kind")] object? kind, [Values(null, "tag")] object? tag)
  {
    if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = Unit.Of(kind, tag);
    var unit2 = Unit.Of(kind, ServiceTag.Any);

    // --assert
    unit1.Equals(unit2).Should().BeFalse();
    unit2.Equals(unit1).Should().BeFalse();
  }
}