using System;
using System.IO;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType;

public class IsAssignableFromTypeTest
{
  [Test]
  public void should_match_if_types_are_same([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), key);
    var target = new IsAssignableFromType(typeof(Stream), key);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_if_specified_type_can_be_assigned_to_the_type_of_unit([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(IDisposable), key);
    var target = new IsAssignableFromType(typeof(Stream), key);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_if_specified_type_cant_be_assigned_to_the_type_of_unit([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), key);
    var target = new IsAssignableFromType(typeof(IDisposable), key);

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_match_for_nullable_struct([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(bool?), key);
    var target = new IsAssignableFromType(typeof(bool), key);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_if_key_is_any([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), key);
    var target = new IsAssignableFromType(typeof(Stream), SpecialKey.Any);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_if_key_differs([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), key);
    var target = new IsAssignableFromType(typeof(Stream), "different key");

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_equal_if_arguments_equal([Values(null, "key")] object? key)
  {
    // --arrange
    var target1 = new IsAssignableFromType(typeof(Stream), key);
    var target2 = new IsAssignableFromType(typeof(Stream), key);

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_equal_if_types_assignable_but_not_equal([Values(null, "key")] object? key)
  {
    // --arrange
    var target1 = new IsAssignableFromType(typeof(IDisposable), key);
    var target2 = new IsAssignableFromType(typeof(Stream), key);

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_not_equal_if_key_differ([Values(null, "key")] object? key)
  {
    // --arrange
    var target1 = new IsAssignableFromType(typeof(Stream), key);
    var target2 = new IsAssignableFromType(typeof(Stream), "different key");

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}