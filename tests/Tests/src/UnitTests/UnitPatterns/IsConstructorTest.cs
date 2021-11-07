using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.UnitPatterns;

public class IsConstructorTest
{
  [Test]
  public void should_match_if_key_is_constructor_and_unit_kind_is_suitable_type(
    [Values(typeof(int), typeof(string), typeof(List<int>), typeof(MemoryStream))]
    Type unitType)
  {
    // --arrange
    var unitId = new UnitId(unitType, SpecialKey.Constructor);
    var target = new IsConstructor();

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_if_key_is_constructor_and_unit_kind_is_suitable_type(
    [Values(typeof(IDisposable), typeof(Stream), typeof(List<>), "not a type", null)]
    object? kind)
  {
    // --arrange
    var unitId = new UnitId(kind, SpecialKey.Constructor);
    var target = new IsConstructor();

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_if_key_is_not_constructor(
    [Values(typeof(int), typeof(string), typeof(List<int>), typeof(MemoryStream))]
    Type unitType)
  {
    // --arrange
    var unitId = new UnitId(unitType, SpecialKey.Any);
    var target = new IsConstructor();

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new IsConstructor();
    var target2 = new IsConstructor();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new IsConstructor();
    var target2 = new Util.OtherUnitPattern();

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}