using System.Collections.Generic;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class UnitInfoMatcherTest
  {
    [Test]
    public void should_match_with_null_id()
    {
      const string key = "key";

      var unitInfo = new UnitId(null, key);
      var matcher  = new UnitIdMatcher(null, key);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_with_null_key()
    {
      const string id = "id";

      var unitInfo = new UnitId(id, null);
      var matcher  = new UnitIdMatcher(id, null);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match()
    {
      const string id  = "id";
      const string key = "key";

      var unitInfo = new UnitId(id, key);
      var matcher  = new UnitIdMatcher(id, key);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_if_key_any_provided([Values(null, "id")] object id)
    {
      var unitInfo = new UnitId(id, "key");
      var matcher  = new UnitIdMatcher(id, UnitKey.Any);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_not_match_if_id_differs([Values(null, "key")] object key)
    {
      var unitInfo = new UnitId("id1", key);
      var matcher  = new UnitIdMatcher("id2", key);

      // --assert
      matcher.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_key_differs([Values(null, "id")] object id)
    {
      var unitInfo = new UnitId(id, "key1");
      var matcher  = new UnitIdMatcher(id, "key2");

      // --assert
      matcher.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_be_equal1([Values(null, "id")] object id)
    {
      const string key = "key";

      var matcher1 = new UnitIdMatcher(id, key);
      var matcher2 = new UnitIdMatcher(id, key);

      // --assert
      matcher1.Equals(matcher2).Should().BeTrue();
      matcher2.Equals(matcher1).Should().BeTrue();
    }

    [TestCaseSource(nameof(KeysSource))]
    public void should_be_equal2(object key)
    {
      const string id = "id";

      var matcher1 = new UnitIdMatcher(id, key);
      var matcher2 = new UnitIdMatcher(id, key);

      // --assert
      matcher1.Equals(matcher2).Should().BeTrue();
      matcher2.Equals(matcher1).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal1([Values(null, "id")] object id)
    {
      var matcher1 = new UnitIdMatcher(id, "key");
      var matcher2 = new UnitIdMatcher(id, UnitKey.Any);

      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }

    [Test]
    public void should_not_be_equal2()
    {
      const string id = "id";

      var matcher1 = new UnitIdMatcher(id, null);
      var matcher2 = new UnitIdMatcher(id, UnitKey.Any);

      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }

    [TestCaseSource(nameof(KeysSource))]
    public void should_not_be_equal3(object key)
    {
      var matcher1 = new UnitIdMatcher("id1", key);
      var matcher2 = new UnitIdMatcher("id2", key);

      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }

    [TestCaseSource(nameof(KeysSource))]
    public void should_not_be_equal4(object key)
    {
      if(key == null) return;

      var matcher1 = new UnitIdMatcher(null, key);
      var matcher2 = new UnitIdMatcher("id2", key);

      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }

    private static IEnumerable<object> KeysSource()
    {
      yield return null;
      yield return "key";
      yield return UnitKey.Any;
    }
  }
}
