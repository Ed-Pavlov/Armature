using System;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class MultiParentTest
  {
    [Test]
    public void should_try_build_value_via_all_parent_builders()
    {
      const string expected = "2";

      // --arrange
      var parent1 = FunctionalTestHelper.CreateBuilder();
      var parent2 = FunctionalTestHelper.CreateBuilder();
      parent2
        .Treat<string>()
        .AsInstance(expected);

      var target = FunctionalTestHelper.CreateBuilder(parent1, parent2);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected, "Expected value registered in one of the parent build (passed last for tesing purpose)");
    }

    [Test]
    public void should_build_value_via_child_builder_first()
    {
      const string value1 = "1";
      const string value2 = "2";
      const string expected = "325";

      // --arrange
      var parent1 = FunctionalTestHelper.CreateBuilder();
      parent1
        .Treat<string>()
        .AsInstance(value1);

      var parent2 = FunctionalTestHelper.CreateBuilder();
      parent2
        .Treat<string>()
        .AsInstance(value2);

      var target = FunctionalTestHelper.CreateBuilder(parent1, parent2);
      target
        .Treat<string>()
        .AsInstance(expected);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected, "Expected value registered in child builder");
    }

    [Test]
    public void should_build_value_via_parent_builders_in_order()
    {
      const string expected = "1";
      const string value2 = "2";

      // --arrange
      var parent1 = FunctionalTestHelper.CreateBuilder();
      parent1
        .Treat<string>()
        .AsInstance(expected);

      var parent2 = FunctionalTestHelper.CreateBuilder();
      parent2
        .Treat<string>()
        .AsInstance(value2);

      var target = FunctionalTestHelper.CreateBuilder(parent1, parent2);

      // --act
      var actual = target.Build<string>();

      // --assert
      actual.Should().Be(expected, "Expected value registered in the parent builder wich passed first");
    }

    [Test]
    public void should_fail_if_there_is_no_registration_in_any_builder()
    {
      // --arrange
      var parent1 = FunctionalTestHelper.CreateBuilder();
      var parent2 = FunctionalTestHelper.CreateBuilder();
      var target = FunctionalTestHelper.CreateBuilder(parent1, parent2);

      // --act
      Action action = () => target.Build<string>();

      // --assert
      action.ShouldThrowExactly<ArmatureException>("There is no registration neigher in child neither in parent builders");
    }
  }
}