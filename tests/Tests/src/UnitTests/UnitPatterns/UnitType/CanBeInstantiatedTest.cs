using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType;

public class CanBeInstantiatedTest
{
  [Test]
  public void should_be_true_for_normal_type(
    [Values(typeof(int), typeof(string), typeof(MemoryStream), typeof(List<List<int[]>>))]
    Type unitType,
    [Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(unitType, tag);
    var target = new CanBeInstantiated();

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_be_false_for_interface_abstract_open_generic_and_no_type(
    [Values(typeof(List<>), typeof(IDisposable), typeof(Stream), "not a type", null)] object? unitType, [Values(null, "tag")] object? tag)
  {
    if(unitType is null && tag is null) Assert.Ignore("Invalid combination of arguments");

    // --arrange
    var unitId = new UnitId(unitType, tag);
    var target = new CanBeInstantiated();

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new CanBeInstantiated();
    var target2 = new CanBeInstantiated();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new CanBeInstantiated();
    var target2 = new Util.OtherUnitPattern();

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}