using System.Collections.Generic;
using Armature.Core;
using Armature.Core.UnitMatchers;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class UnitInfoMatcherTest
  {
   [Test]
    public void should_match_with_null_id()
    {
      const string token = "token";
      
      var unitInfo = new UnitInfo(null, token);
      var matcher = new UnitInfoMatcher(null, token);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_with_null_token()
    {
      const string id = "id";
      
      var unitInfo = new UnitInfo(id, null);
      var matcher = new UnitInfoMatcher(id, null);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match()
    {
      const string id = "id";
      const string token = "token";
      
      var unitInfo = new UnitInfo(id, token);
      var matcher = new UnitInfoMatcher(id, token);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_match_if_token_any_provided([Values(null, "id")] object id)
    {
      var unitInfo = new UnitInfo(id, "token");
      var matcher = new UnitInfoMatcher(id, Token.Any);

      // --assert
      matcher.Matches(unitInfo).Should().BeTrue();
    }

    [Test]
    public void should_not_match_if_id_differs([Values(null, "token")] object token)
    {
      var unitInfo = new UnitInfo("id1", token);
      var matcher = new UnitInfoMatcher("id2", token);

      // --assert
      matcher.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_not_match_if_token_differs([Values(null, "id")] object id)
    {
      var unitInfo = new UnitInfo(id, "token1");
      var matcher = new UnitInfoMatcher(id, "token2");

      // --assert
      matcher.Matches(unitInfo).Should().BeFalse();
    }

    [Test]
    public void should_be_equal1([Values(null, "id")] object id)
    {
      const string token = "token";
      
      var matcher1 = new UnitInfoMatcher(id, token);
      var matcher2 = new UnitInfoMatcher(id, token);
      
      // --assert
      matcher1.Equals(matcher2).Should().BeTrue();
      matcher2.Equals(matcher1).Should().BeTrue();
    }

    [TestCaseSource(nameof(TokensSource))]
    public void should_be_equal2(object token)
    {
      const string id = "id";
      
      var matcher1 = new UnitInfoMatcher(id, token);
      var matcher2 = new UnitInfoMatcher(id, token);
      
      // --assert
      matcher1.Equals(matcher2).Should().BeTrue();
      matcher2.Equals(matcher1).Should().BeTrue();
    }

    [Test]
    public void should_not_be_equal1([Values(null, "id")] object id)
    {
      var matcher1 = new UnitInfoMatcher(id, "token");
      var matcher2 = new UnitInfoMatcher(id, Token.Any);
      
      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }
    
    [Test]
    public void should_not_be_equal2()
    {
      const string id = "id";
      
      var matcher1 = new UnitInfoMatcher(id, null);
      var matcher2 = new UnitInfoMatcher(id, Token.Any);
      
      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }
    
    [TestCaseSource(nameof(TokensSource))]
    public void should_not_be_equal3(object token)
    {
      var matcher1 = new UnitInfoMatcher("id1", token);
      var matcher2 = new UnitInfoMatcher("id2", token);
      
      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }
    
    [TestCaseSource(nameof(TokensSource))]
    public void should_not_be_equal4(object token)
    {
      if (token == null) return;
      
      var matcher1 = new UnitInfoMatcher(null, token);
      var matcher2 = new UnitInfoMatcher("id2", token);
      
      // --assert
      matcher1.Equals(matcher2).Should().BeFalse();
      matcher2.Equals(matcher1).Should().BeFalse();
    }
    
    private static IEnumerable<object> TokensSource()
    {
      yield return null;
      yield return "token";
      yield return Token.Any;
    }
  }
}