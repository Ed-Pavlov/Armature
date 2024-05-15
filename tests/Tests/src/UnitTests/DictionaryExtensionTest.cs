using System;
using System.Collections.Generic;
using Armature.Core;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
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

      // --arrange
      var target = new Dictionary<int, string> {{1, expected}, {2, "two"}};

      // --act
      var actual = DictionaryExtension.GetValueSafe(target, 1);

      // ----assert
      actual.Should().Be(expected);
    }

    [Test]
    public void should_return_null_if_no_value()
    {
      var target = new Dictionary<int, string> {{1, "one"}, {2, "two"}};

      // --act
      var actual = DictionaryExtension.GetValueSafe(target, 3);

      // ----assert
      actual.Should().BeNull();
    }

    [Test]
    public void should_add_null_as_value()
    {
      var target = new Dictionary<int, string?> {{1, "one"}, {2, "two"}};

      // --act
      var actual = DictionaryExtension.GetOrCreateValue(target, 3, () => null);

      // ----assert
      actual.Should().BeNull();
      target[3].Should().BeNull();
    }

    [Test]
    public void should_return_default_if_no_value()
    {
      var target = new Dictionary<int, Weighted<string>> {{1, "one".WithWeight(1)}, {2, "two".WithWeight(2)}};

      // --act
      var actual = DictionaryExtension.GetValueSafe(target, 3);

      // ----assert
      actual.Should().Be(default);
    }

    [Test]
    public void GetValueSafe_should_check_arguments()
    {
      var nullDictionary = () => DictionaryExtension.GetValueSafe(null!, "key", "value");
      nullDictionary.Should().ThrowExactly<ArgumentNullException>().WithParameterName("dictionary");

      var  target = new Dictionary<string, string>();

      var nullKey = () => DictionaryExtension.GetValueSafe(target, null!, "value");
      nullKey.Should().ThrowExactly<ArgumentNullException>().WithParameterName("key");
    }
    [Test]
    public void GetOrCreateValue_should_check_arguments()
    {
      var nullDictionary = () => DictionaryExtension.GetOrCreateValue(null!, "key", () => "value");
      nullDictionary.Should().ThrowExactly<ArgumentNullException>().WithParameterName("dictionary");

      var  target = new Dictionary<string, string>();

      var nullKey = () => DictionaryExtension.GetOrCreateValue(target, null!, () => "value");
      nullKey.Should().ThrowExactly<ArgumentNullException>().WithParameterName("key");

      var nullFactory = () => DictionaryExtension.GetOrCreateValue(target, "key", null!);
      nullFactory.Should().ThrowExactly<ArgumentNullException>().WithParameterName("createValue");
    }
  }
}