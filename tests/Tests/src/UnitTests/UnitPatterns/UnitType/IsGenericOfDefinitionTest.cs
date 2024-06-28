using System;
using System.Collections.Generic;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType;

public class IsGenericOfDefinitionTest
{
  [Test]
  public void should_match_open_generic_type([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = Unit.By(typeof(List<>), tag);
    var target = new IsGenericOfDefinition(typeof(List<>), tag);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_generic_type([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = Unit.By(typeof(List<int>), tag);
    var target = new IsGenericOfDefinition(typeof(List<>), tag);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_not_equal_open_generic_type([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = Unit.By(typeof(IList<>), tag);
    var target = new IsGenericOfDefinition(typeof(List<>), tag);

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_match_if_tag_is_any([Values(null, "tag")] object? tag)
  {
    // --arrange
    var unitId = Unit.By(typeof(List<int>), tag);
    var target = new IsGenericOfDefinition(typeof(List<>), ServiceTag.Any);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_allow_null_type_argument()
  {
    // --arrange
    var target = () => new IsGenericOfDefinition(null!, null);

    // --assert
    target.Should().ThrowExactly<ArgumentNullException>().WithParameterName("type");
  }

  [Test]
  public void should_equal_if_arguments_equal([Values(null, "tag")] object? tag)
  {
    // --arrange
    var target1 = new IsGenericOfDefinition(typeof(List<>), tag);
    var target2 = new IsGenericOfDefinition(typeof(List<>), tag);

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_equal_if_type_differ([Values(null, "tag")] object? tag)
  {
    // --arrange
    var target1 = new IsGenericOfDefinition(typeof(IList<>), tag);
    var target2 = new IsGenericOfDefinition(typeof(List<>), tag);

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_not_equal_if_tag_differ([Values(null, "tag")] object? tag)
  {
    // --arrange
    var target1 = new IsGenericOfDefinition(typeof(List<>), tag);
    var target2 = new IsGenericOfDefinition(typeof(List<>), "different tag");

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_check_argument([Values(null, "tag")] object? tag)
  {
    // --arrange
    var actual = () => new IsGenericOfDefinition(typeof(List<int>), tag);

    // --assert
    actual.Should().ThrowExactly<ArgumentException>().WithParameterName("genericTypeDefinition").WithMessage("Should be an open generic type*");
  }
}