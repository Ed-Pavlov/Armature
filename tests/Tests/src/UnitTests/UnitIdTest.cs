using System;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests;

public class UnitIdTest
{
  [Test]
  public void kind_or_tag_should_be_not_null()
  {
    // --arrange
    var actual = () => new UnitId(null, null);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Either kind or tag should be provided*");
  }

  [Test]
  public void should_be_equal([Values(null, "kind")] object? kind, [Values(null, "tag")] object? tag)
  {
    if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = new UnitId(kind, tag);
    var unit2 = new UnitId(kind, tag);

    // --assert
    unit1.Equals(unit2).Should().BeTrue();
    unit2.Equals(unit1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal([Values(null, "kind")] object? kind, [Values(null, "tag")] object? tag)
  {
    if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = new UnitId(tag, kind);
    var unit2 = new UnitId(kind, tag);

    // --assert
    unit1.Equals(unit2).Should().BeFalse();
    unit2.Equals(unit1).Should().BeFalse();
  }

  [Test]
  public void should_ignore_special_tag([Values(null, "kind")] object? kind, [Values(null, "tag")] object? tag)
  {
    if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = new UnitId(kind, tag);
    var unit2 = new UnitId(kind, Tag.Any);

    // --assert
    unit1.Equals(unit2).Should().BeFalse();
    unit2.Equals(unit1).Should().BeFalse();
  }
}