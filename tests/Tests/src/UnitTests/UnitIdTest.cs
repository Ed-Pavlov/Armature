using System;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests;

public class UnitIdTest
{
  [Test]
  public void kind_or_key_should_be_not_null()
  {
    // --arrange
    var actual = () => new UnitId(null, null);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithMessage($"Either kind or key should be provided*");
  }

  [Test]
  public void should_be_equal([Values(null, "kind")] object? kind, [Values(null, "key")] object? key)
  {
    if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = new UnitId(kind, key);
    var unit2 = new UnitId(kind, key);

    // --assert
    unit1.Equals(unit2).Should().BeTrue();
    unit2.Equals(unit1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal([Values(null, "kind")] object? kind, [Values(null, "key")] object? key)
  {
    if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = new UnitId(key, kind);
    var unit2 = new UnitId(kind, key);

    // --assert
    unit1.Equals(unit2).Should().BeFalse();
    unit2.Equals(unit1).Should().BeFalse();
  }

  [Test]
  public void should_ignore_special_key([Values(null, "kind")] object? kind, [Values(null, "key")] object? key)
  {
    if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

    // --arrange
    var unit1 = new UnitId(kind, key);
    var unit2 = new UnitId(kind, SpecialKey.Any);

    // --assert
    unit1.Equals(unit2).Should().BeFalse();
    unit2.Equals(unit1).Should().BeFalse();
  }
}