using System;
using System.Collections.Generic;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns
{
  public class UnitPatternTest
  {
    [Test]
    public void should_match([Values(null, "kind")] string? kind, [Values(null, "key")] string? key )
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var unitInfo = new UnitId(kind, key);
      var target  = new UnitPattern(kind, key);

      // --assert
      target.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_if_key_any_provided([Values(null, "key")] object key)
    {
      var unitInfo = new UnitId("kind", key);
      var target  = new UnitPattern("kind", SpecialKey.Any);

      // --assert
      target.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_not_match_if_kind_differs([Values(null, "key")] object key)
    {
      var unitInfo = new UnitId("kind1", key);
      var target  = new UnitPattern("kind2", key);

      // --assert
      target.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_key_differs([Values(null, "kind")] object kind)
    {
      var unitInfo = new UnitId(kind, "key1");
      var target  = new UnitPattern(kind, "key2");

      // --assert
      target.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_be_equal_if_kind_and_key_are_equal([Values(null, "kind")] string? kind, [ValueSource(nameof(all_possible_keys))] object? key)
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var target1 = new UnitPattern(kind, key);
      var target2 = new UnitPattern(kind, key);

      // --assert
      target1.Equals(target2).Should().BeTrue();
      target2.Equals(target1).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal_if_keys_are_not_equal([Values(null, "kind")] object? kind, [Values(null, "key")] string? key)
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var target1 = new UnitPattern(kind, key);
      var target2 = new UnitPattern(kind, SpecialKey.Any); // fix key value

      // --assert
      target1.Equals(target2).Should().BeFalse();
      target2.Equals(target1).Should().BeFalse();
    }

    [Test]
    public void should_not_be_equal_if_kinds_are_not_equal([Values(null, "kind")] object? kind, [ValueSource(nameof(all_possible_keys))] object? key)
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var target1 = new UnitPattern(kind, key);
      var target2 = new UnitPattern("fixed kind", key); // fix kind value

      // --assert
      target1.Equals(target2).Should().BeFalse();
      target2.Equals(target1).Should().BeFalse();
    }

    [Test]
    public void should_throw_if_both_arguments_are_null()
    {
      var actual = () => new UnitPattern(null, null);

      actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("unitKind").WithMessage("Either unit kind or key should be provided*");
    }

    public static IEnumerable<object?> all_possible_keys()
    {
      yield return null;
      yield return "key"; // object
      yield return 4; // value type
      yield return SpecialKey.Any; // special
    }
  }
}