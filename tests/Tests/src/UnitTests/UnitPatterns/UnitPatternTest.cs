using System;
using System.Collections.Generic;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns
{
  public class UnitPatternTest
  {
    [Test]
    public void should_match([Values(null, "kind")] string? kind, [Values(null, "tag")] string? tag )
    {
      if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

      var unitInfo = Unit.Of(kind, tag);
      var target  = new UnitPattern(kind, tag);

      // --assert
      target.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_if_tag_any_provided([Values(null, "tag")] object tag)
    {
      var unitInfo = Unit.Of("kind", tag);
      var target  = new UnitPattern("kind", ServiceTag.Any);

      // --assert
      target.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_not_match_if_kind_differs([Values(null, "tag")] object tag)
    {
      var unitInfo = Unit.Of("kind1", tag);
      var target  = new UnitPattern("kind2", tag);

      // --assert
      target.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_tag_differs([Values(null, "kind")] object kind)
    {
      var unitInfo = Unit.Of(kind, "tag1");
      var target  = new UnitPattern(kind, "tag2");

      // --assert
      target.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_be_equal_if_kind_and_tag_are_equal([Values(null, "kind")] string? kind, [ValueSource(nameof(all_possible_tags))] object? tag)
    {
      if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

      var target1 = new UnitPattern(kind, tag);
      var target2 = new UnitPattern(kind, tag);

      // --assert
      target1.Equals(target2).Should().BeTrue();
      target2.Equals(target1).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal_if_tags_are_not_equal([Values(null, "kind")] object? kind, [Values(null, "tag")] string? tag)
    {
      if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

      var target1 = new UnitPattern(kind, tag);
      var target2 = new UnitPattern(kind, ServiceTag.Any); // fix tag value

      // --assert
      target1.Equals(target2).Should().BeFalse();
      target2.Equals(target1).Should().BeFalse();
    }

    [Test]
    public void should_not_be_equal_if_kinds_are_not_equal([Values(null, "kind")] object? kind, [ValueSource(nameof(all_possible_tags))] object? tag)
    {
      if(kind is null && tag is null) Assert.Ignore("Impossible arguments combination");

      var target1 = new UnitPattern(kind, tag);
      var target2 = new UnitPattern("fixed kind", tag); // fix kind value

      // --assert
      target1.Equals(target2).Should().BeFalse();
      target2.Equals(target1).Should().BeFalse();
    }

    [Test]
    public void should_throw_if_both_arguments_are_null()
    {
      var actual = () => new UnitPattern(null);

      actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("unitKind").WithMessage("Either unitKind or tag should be provided*");
    }

    public static IEnumerable<object?> all_possible_tags()
    {
      yield return null;
      yield return "tag"; // object
      yield return 4; // value type
      yield return ServiceTag.Any; // special
    }
  }
}