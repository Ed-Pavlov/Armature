using System;
using System.Collections;
using System.IO;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.UnitType
{
  public class IsInheritorOfTest
  {
    [Test]
    public void should_not_match_exact_type([Values(null, "key")] object key)
    {
      var unitId = new UnitId(typeof(Stream), key);
      var target = new IsInheritorOf(typeof(Stream), key);

      target.Matches(unitId).Should().BeFalse();
    }

    [Test]
    public void should_match_base_type([Values(null, "key")] object key)
    {
      var unitId = new UnitId(typeof(MemoryStream), key);
      var target = new IsInheritorOf(typeof(Stream), key);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_match_interface([Values(null, "key")] object key)
    {
      var unitId = new UnitId(typeof(MemoryStream), key);
      var target = new IsInheritorOf(typeof(IDisposable), key);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_match_value_type_as_inheritor_of_object([Values(null, "key")] object key)
    {
      var unitId = new UnitId(typeof(int), key);
      var target = new IsInheritorOf(typeof(object), key);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_match_any_key_if_specified([Values(null, "key")] object key)
    {
      var unitId = new UnitId(typeof(MemoryStream), key);
      var target = new IsInheritorOf(typeof(Stream), SpecialKey.Any);

      target.Matches(unitId).Should().BeTrue();
    }

    [Test]
    public void should_not_match_not_inheritor([Values(null, "key")] object key)
    {
      var unitId = new UnitId(typeof(MemoryStream), key);
      var target = new IsInheritorOf(typeof(IEnumerable), key);

      target.Matches(unitId).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_key_differs([Values(null, "unitKey")] object unitKey, [Values(null, "patternKey")] object? patternKey)
    {
      if(Equals(unitKey, patternKey)) Assert.Ignore("Not the case");

      var unitId = new UnitId(typeof(MemoryStream), unitKey);
      var target = new IsInheritorOf(typeof(Stream), patternKey);

      target.Matches(unitId).Should().BeFalse();
    }

    [Test]
    public void should_be_equal_if_fields_equal([Values(null, "key")] object key)
    {
      var target1 = new IsInheritorOf(typeof(Stream), key);
      var target2 = new IsInheritorOf(typeof(Stream), key);

      target1.Equals(target2).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal_if_type_differs([Values(null, "key")] object key)
    {
      var target1 = new IsInheritorOf(typeof(Stream), key);
      var target2 = new IsInheritorOf(typeof(MemoryStream), key);

      target1.Equals(target2).Should().BeFalse();
    }

    [Test]
    public void should_not_be_equal_if_key_differs([Values(null, "key")] object key)
    {
      var target1 = new IsInheritorOf(typeof(Stream), key);
      var target2 = new IsInheritorOf(typeof(Stream), "different key");

      target1.Equals(target2).Should().BeFalse();
    }

    [Test]
    public void should_throw_if_type_is_null([Values(null, "key")] object key)
    {
      var actual = () => new IsInheritorOf(null!, key);

      actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("Type");
    }
  }
}