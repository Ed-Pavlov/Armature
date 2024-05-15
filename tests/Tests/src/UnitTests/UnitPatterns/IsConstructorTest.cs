using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.UnitPatterns;

public class IsConstructorTest
{
  [Test]
  public void should_match_if_tag_is_constructor(
    [Values(typeof(int), typeof(string), typeof(List<int>), typeof(List<>), null, "not a type at all")]
    object? kind)
  {
    // --arrange
    var unitId = Unit.Of(kind, ServiceTag.Constructor);
    var target = new IsConstructor();

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_if_tag_is_not_constructor(
    [Values(typeof(int), typeof(string), typeof(List<int>), typeof(MemoryStream))]
    Type unitType)
  {
    // --arrange
    var unitId = Unit.Of(unitType, ServiceTag.Any);
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
    var target2 = new TestUtil.OtherUnitPattern();

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}