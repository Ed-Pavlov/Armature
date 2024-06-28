using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsParameterArgumentTest
{
  [Test]
  public void should_match_parameter_info_with_argument_tag()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;

    // --arrange
    var unitId = Unit.By(parameterInfo, ServiceTag.Argument);
    var target = new IsParameterArgument();

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_unit_kind_other_than_parameter_info()
  {
    // --arrange
    var unitId = Unit.By("parameterInfo", ServiceTag.Argument);
    var target = new IsParameterArgument();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_parameter_info_with_not_argument_tag([Values(null, "tag")] object? tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;

    // --arrange
    var unitId = Unit.By(parameterInfo, tag);
    var target = new IsParameterArgument();

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void all_instances_should_be_equal()
  {
    // --arrange
    var target1 = new IsParameterArgument();
    var target2 = new IsParameterArgument();

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
  [Test]
  public void should_not_be_equal_to_other_unit_patterns()
  {
    // --arrange
    var target1 = new IsParameterArgument();
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