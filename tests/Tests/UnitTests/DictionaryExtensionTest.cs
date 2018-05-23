using System.Collections.Generic;
using Armature.Core;
using Armature.Core.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class DictionaryExtensionTest
  {
    [Test]
    public void should_return_value()
    {
      const string expected = "one";
      var target = new Dictionary<int, string> {{1, expected}, {2, "two"}};

      // --act 
      var actual = target.GetValueSafe(1);

      // ----assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_return_null_if_no_value()
    {
      var target = new Dictionary<int, string> {{1, "one"}, {2, "two"}};

      // --act 
      var actual = target.GetValueSafe(3);

      // ----assert
      actual.Should().BeNull();
    }

    [Test]
    public void should_return_default_if_no_value()
    {
      var target = new Dictionary<int, Weighted<string>> {{1, "one".WithWeight(1)}, {2, "two".WithWeight(2)}};

      // --act 
      var actual = target.GetValueSafe(3);

      // ----assert
      actual.Should().Be(default(Weighted<string>));
    }
  }
}