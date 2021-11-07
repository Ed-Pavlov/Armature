using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns;

public class IsServiceUnitTest
{
  [Test]
  public void should_match_any_kind(
    [Values(null, "kind", typeof(string))]                     object?    kind,
    [ValueSource(typeof(Util), nameof(Util.all_special_keys))] SpecialKey key)
  {
    // --arrange
    var unitId = new UnitId(kind, key);
    var target = new IsServiceUnit();

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_any_kind_if_key_is_not_special(
    [Values(null, "kind", typeof(string))] object? kind,
    [Values(null, "key")]                  object? key)
  {
    if(kind is null && key is null) Assert.Ignore("Impossible argument combination");

    // --arrange
    var unitId = new UnitId(kind, key);
    var target = new IsServiceUnit();

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new IsServiceUnit();
    var target2 = new IsServiceUnit();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new IsServiceUnit();
    var target2 = new Util.OtherUnitPattern();

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}