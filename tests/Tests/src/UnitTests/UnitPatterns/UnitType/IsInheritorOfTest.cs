﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType
{
  public class IsInheritorOfTest
  {
    [Test]
    public void should_not_match_exact_type([Values(null, "tag")] object tag)
    {
      var unitId = Unit.By(typeof(Stream), tag);
      var target = new IsInheritorOf(typeof(Stream), tag);

      target.Matches(unitId).Should().BeFalse();
    }

    [Test]
    public void should_match_base_type([Values(null, "tag")] object tag)
    {
      var unitId = Unit.By(typeof(MemoryStream), tag);
      var target = new IsInheritorOf(typeof(Stream), tag);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_match_interface([Values(null, "tag")] object tag)
    {
      var unitId = Unit.By(typeof(MemoryStream), tag);
      var target = new IsInheritorOf(typeof(IDisposable), tag);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_match_value_type_as_inheritor_of_object([Values(null, "tag")] object tag)
    {
      var unitId = Unit.By(typeof(int), tag);
      var target = new IsInheritorOf(typeof(object), tag);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_match_any_tag_if_specified([Values(null, "tag")] object tag)
    {
      var unitId = Unit.By(typeof(MemoryStream), tag);
      var target = new IsInheritorOf(typeof(Stream), ServiceTag.Any);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_not_match_not_inheritor([Values(null, "tag")] object tag)
    {
      var unitId = Unit.By(typeof(MemoryStream), tag);
      var target = new IsInheritorOf(typeof(IEnumerable), tag);

      target.Matches(unitId).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_tag_differs([Values(null, "unitTag")] object unitTag, [Values(null, "patternTag")] object? patternTag)
    {
      if(Equals(unitTag, patternTag)) Assert.Ignore("Not the case");

      var unitId = Unit.By(typeof(MemoryStream), unitTag);
      var target = new IsInheritorOf(typeof(Stream), patternTag);

      target.Matches(unitId).Should().BeFalse();
    }

    [Test]
    public void should_be_equal_if_fields_equal([Values(null, "tag")] object tag)
    {
      var target1 = new IsInheritorOf(typeof(Stream), tag);
      var target2 = new IsInheritorOf(typeof(Stream), tag);

      target1.Equals(target2).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal_if_type_differs([Values(null, "tag")] object tag)
    {
      var target1 = new IsInheritorOf(typeof(Stream), tag);
      var target2 = new IsInheritorOf(typeof(MemoryStream), tag);

      target1.Equals(target2).Should().BeFalse();
    }

    [Test]
    public void should_not_be_equal_if_tag_differs([Values(null, "tag")] object tag)
    {
      var target1 = new IsInheritorOf(typeof(Stream), tag);
      var target2 = new IsInheritorOf(typeof(Stream), "different tag");

      target1.Equals(target2).Should().BeFalse();
    }

    [Test]
    public void should_throw_if_type_is_null([Values(null, "tag")] object tag)
    {
      var actual = () => new IsInheritorOf(null!, tag);

      actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("type");
    }

    [Test]
    public void should_throw_if_type_is_open_generic([Values(null, "tag")] object tag)
    {
      var actual = () => new IsInheritorOf(typeof(IList<>), tag);

      actual.Should().ThrowExactly<ArgumentException>().WithParameterName("type");
    }
  }
}