using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsParameterInfoListTest
{
  [Test]
  public void should_match_parameter_info_with_argument_key()
  {
    var parameterInfoList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters();

    // --arrange
    var unitId = new UnitId(parameterInfoList, SpecialKey.Argument);
    var target = new IsParameterInfoList();

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_unit_kind_other_than_parameter_info()
  {
    // --arrange
    var unitId = new UnitId("parameterInfoList", SpecialKey.Argument);
    var target = new IsParameterInfoList();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_parameter_info_with_not_argument_key([Values(null, "key")] object? key)
  {
    var parameterInfoList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters();

    // --arrange
    var unitId = new UnitId(parameterInfoList, key);
    var target = new IsParameterInfoList();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new IsParameterInfoList();
    var target2 = new IsParameterInfoList();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new IsParameterInfoList();
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