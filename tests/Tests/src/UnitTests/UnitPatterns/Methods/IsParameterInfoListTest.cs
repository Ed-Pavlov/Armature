using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsParameterInfoListTest
{
  [Test]
  public void should_match_parameter_info_with_argument_tag()
  {
    var parameterInfoList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters();

    // --arrange
    var unitId = new UnitId(parameterInfoList, SpecialTag.Argument);
    var target = new IsParameterInfoArray();

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_unit_kind_other_than_parameter_info()
  {
    // --arrange
    var unitId = new UnitId("parameterInfoList", SpecialTag.Argument);
    var target = new IsParameterInfoArray();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_parameter_info_with_not_argument_tag([Values(null, "tag")] object? tag)
  {
    var parameterInfoList = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters();

    // --arrange
    var unitId = new UnitId(parameterInfoList, tag);
    var target = new IsParameterInfoArray();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new IsParameterInfoArray();
    var target2 = new IsParameterInfoArray();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new IsParameterInfoArray();
    var target2 = new TestUtil.OtherUnitPattern();

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