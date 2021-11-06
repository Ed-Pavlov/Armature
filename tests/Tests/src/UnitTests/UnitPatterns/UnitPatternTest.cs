using System;
using System.Collections.Generic;
using Armature.Core;
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
      var matcher  = new Pattern(kind, key);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_if_key_any_provided([Values(null, "key")] object key)
    {
      var unitInfo = new UnitId("kind", key);
      var matcher  = new Pattern("kind", SpecialKey.Any);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_not_match_if_kind_differs([Values(null, "key")] object key)
    {
      var unitInfo = new UnitId("kind1", key);
      var matcher  = new Pattern("kind2", key);

      // --assert
      matcher.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_key_differs([Values(null, "kind")] object kind)
    {
      var unitInfo = new UnitId(kind, "key1");
      var matcher  = new Pattern(kind, "key2");

      // --assert
      matcher.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_be_equal_if_kind_and_key_are_equal([Values(null, "kind")] string? kind, [ValueSource(nameof(all_possible_keys))] object? key)
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var matcher1 = new Pattern(kind, key);
      var matcher2 = new Pattern(kind, key);

      // --assert
      matcher1.Equals(matcher2).Should().BeTrue();
      matcher2.Equals(matcher1).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal_if_keys_are_not_equal([Values(null, "kind")] object? kind, [Values(null, "key")] string? key)
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var matcher1 = new Pattern(kind, key);
      var matcher2 = new Pattern(kind, SpecialKey.Any); // fix key value

      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }

    [Test]
    public void should_not_be_equal_if_kinds_are_not_equal([Values(null, "kind")] object? kind, [ValueSource(nameof(all_possible_keys))] object? key)
    {
      if(kind is null && key is null) Assert.Ignore("Impossible arguments combination");

      var matcher1 = new Pattern(kind, key);
      var matcher2 = new Pattern("fixed kind", key); // fix kind value

      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }

    [Test]
    public void should_throw_if_both_arguments_are_null()
    {
      var actual = () => new Pattern(null, null);

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