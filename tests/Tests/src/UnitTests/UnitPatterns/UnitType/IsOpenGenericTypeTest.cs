using System;
using System.Collections.Generic;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType;

public class IsGenericTypeOfGenericDefinitionTest
{
  [Test]
  public void should_match_open_generic_type([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(List<>), key);
    var target = new IsGenericTypeDefinition(typeof(List<>), key);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_generic_type([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(List<int>), key);
    var target = new IsGenericTypeDefinition(typeof(List<>), key);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_not_equal_open_generic_type([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(IList<>), key);
    var target = new IsGenericTypeDefinition(typeof(List<>), key);

    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_match_if_key_is_any([Values(null, "key")] object? key)
  {
    // --arrange
    var unitId = new UnitId(typeof(List<int>), key);
    var target = new IsGenericTypeDefinition(typeof(List<>), SpecialKey.Any);

    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_allow_null_type_argument()
  {
    // --arrange
    var target = () => new IsGenericTypeDefinition(null!, null);

    // --assert
    target.Should().ThrowExactly<ArgumentNullException>().WithParameterName("Type");
  }

  [Test]
  public void should_equal_if_arguments_equal([Values(null, "key")] object? key)
  {
    // --arrange
    var target1 = new IsGenericTypeDefinition(typeof(List<>), key);
    var target2 = new IsGenericTypeDefinition(typeof(List<>), key);

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_equal_if_type_differ([Values(null, "key")] object? key)
  {
    // --arrange
    var target1 = new IsGenericTypeDefinition(typeof(IList<>), key);
    var target2 = new IsGenericTypeDefinition(typeof(List<>), key);

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_not_equal_if_key_differ([Values(null, "key")] object? key)
  {
    // --arrange
    var target1 = new IsGenericTypeDefinition(typeof(List<>), key);
    var target2 = new IsGenericTypeDefinition(typeof(List<>), "different key");

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }
}