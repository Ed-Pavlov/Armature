using System;
using System.IO;
using Armature.Core.UnitMatchers;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests
{
  public class BaseTypeMatcherTest
  {
    [Test]
    public void should_match_exact_type([Values(null, "token")] object token)
    {
      var target = new BaseTypeMatcher(typeof(int), token);

      target.Matches(Unit.OfType<int>(token)).Should().BeTrue();
    }

    [Test]
    public void should_match_base_type([Values(null, "token")] object token)
    {
      var target = new BaseTypeMatcher(typeof(Stream), token);

      target.Matches(Unit.OfType<MemoryStream>(token)).Should().BeTrue();
    }

    [Test]
    public void should_match_interface([Values(null, "token")] object token)
    {
      var target = new BaseTypeMatcher(typeof(IDisposable), token);

      target.Matches(Unit.OfType<MemoryStream>(token)).Should().BeTrue();
    }
  }
}