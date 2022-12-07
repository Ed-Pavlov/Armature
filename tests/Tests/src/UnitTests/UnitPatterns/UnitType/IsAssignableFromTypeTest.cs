using System;
using System.IO;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType;

public class IsAssignableFromTypeTest
{
  [Test]
  public void should_match_if_types_are_same([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), tag);
    var target = new IsAssignableFromType(typeof(Stream), tag);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_if_specified_type_can_be_assigned_to_the_type_of_unit([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(typeof(IDisposable), tag);
    var target = new IsAssignableFromType(typeof(Stream), tag);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_if_specified_type_cant_be_assigned_to_the_type_of_unit([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), tag);
    var target = new IsAssignableFromType(typeof(IDisposable), tag);

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_match_for_nullable_struct([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(typeof(bool?), tag);
    var target = new IsAssignableFromType(typeof(bool), tag);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_if_tag_is_any([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), tag);
    var target = new IsAssignableFromType(typeof(Stream), Tag.Any);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_if_tag_differs([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = new UnitId(typeof(Stream), tag);
    var target = new IsAssignableFromType(typeof(Stream), "different tag");

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_equal_if_arguments_equal([Values(null, "tag")] object? tag)
  {
    // --arrange
    var target1 = new IsAssignableFromType(typeof(Stream), tag);
    var target2 = new IsAssignableFromType(typeof(Stream), tag);

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_equal_if_types_assignable_but_not_equal([Values(null, "tag")] object? tag)
  {
    // --arrange
    var target1 = new IsAssignableFromType(typeof(IDisposable), tag);
    var target2 = new IsAssignableFromType(typeof(Stream), tag);

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_not_equal_if_tag_differ([Values(null, "tag")] object? tag)
  {
    // --arrange
    var target1 = new IsAssignableFromType(typeof(Stream), tag);
    var target2 = new IsAssignableFromType(typeof(Stream), "different tag");

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}