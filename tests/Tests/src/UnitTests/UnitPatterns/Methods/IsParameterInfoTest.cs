using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsParameterInfoTest
{
  [Test]
  public void should_match_parameter_info_with_argument_key()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;

    // --arrange
    var unitId = new UnitId(parameterInfo, SpecialKey.Argument);
    var target = new IsParameterInfo();

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_unit_kind_other_than_parameter_info()
  {
    // --arrange
    var unitId = new UnitId("parameterInfo", SpecialKey.Argument);
    var target = new IsParameterInfo();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_parameter_info_with_not_argument_key([Values(null, "key")] object? key)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;

    // --arrange
    var unitId = new UnitId(parameterInfo, key);
    var target = new IsParameterInfo();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new IsParameterInfo();
    var target2 = new IsParameterInfo();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new IsParameterInfo();
    var target2 = new Util.OtherUnitPattern();

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public static void Foo(int i){}
  }
}