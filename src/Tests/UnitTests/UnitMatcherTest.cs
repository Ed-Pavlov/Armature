using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class UnitMatcherTest
  {
    [Test]
    public void should_be_equal_with_null_id()
    {
      var left = new UnitInfo(null, "token");
      var right = new UnitInfo(null, "token");

      Equals(left, right).Should().BeTrue();
      Equals(right, left).Should().BeTrue();
    }

    [Test]
    public void should_be_equal_with_null_token()
    {
      var left = new UnitInfo("id", null);
      var right = new UnitInfo("id", null);

      Equals(left, right).Should().BeTrue();
      Equals(right, left).Should().BeTrue();
    }

    [Test]
    public void should_be_equal()
    {
      var left = new UnitInfo("id", "token");
      var right = new UnitInfo("id", "token");

      Equals(left, right).Should().BeTrue();
      Equals(right, left).Should().BeTrue();
    }

    [Test]
    public void should_be_equal_if_token_any_provided()
    {
      var left = new UnitInfo(null, "token");
      var right = new UnitInfo(null, Token.Any);

      Equals(left, right).Should().BeTrue();
      Equals(right, left).Should().BeTrue();
    }
    
    [Test]
    public void should_not_be_equal_if_id_differs([Values(null, "token")] object token)
    {
      var left = new UnitInfo("id1", token);
      var right = new UnitInfo("id2", token);

      Equals(left, right).Should().BeFalse();
      Equals(right, left).Should().BeFalse();
    }
    
    [Test]
    public void should_not_be_equal_if_token_differs([Values(null, "id")] object id)
    {
      var left = new UnitInfo(id, "token1");
      var right = new UnitInfo(id, "token2");

      Equals(left, right).Should().BeFalse();
      Equals(right, left).Should().BeFalse();
    }
  }
}