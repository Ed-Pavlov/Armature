using System;
using System.IO;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;
using Tests.UnitTests.BuildActions;

namespace Tests.UnitTests
{
  public class UnitIsSubTypeOfMatcherTest
  {
    [Test]
    public void should_match_exact_type([Values(null, "key")] object key)
    {
      var target = new IsSubtypeOf(typeof(int), key);

      target.Matches(Unit.IsType<int>().Key(key)).Should().BeTrue();
    }

    [Test]
    public void should_match_base_type([Values(null, "key")] object key)
    {
      var target = new IsSubtypeOf(typeof(Stream), key);

      target.Matches(Unit.IsType<MemoryStream>().Key(key)).Should().BeTrue();
    }

    [Test]
    public void should_match_interface([Values(null, "key")] object key)
    {
      var target = new IsSubtypeOf(typeof(IDisposable), key);

      target.Matches(Unit.IsType<MemoryStream>().Key(key)).Should().BeTrue();
    }
  }
}