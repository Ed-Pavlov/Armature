using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsParameterMarkedWithAttributeTest
{
  [Test]
  public void should_match_parameter_with_null_point_id()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;

    // --arrange
    var unitId = new UnitId(parameterInfo, SpecialTag.Argument);
    var target = new IsParameterMarkedWithAttribute();

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_parameter_with_point_id()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(string))!;

    // --arrange
    var unitId = new UnitId(parameterInfo, SpecialTag.Argument);
    var target = new IsParameterMarkedWithAttribute(Subject.StringPointId);

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_parameter_without_point_id([Values(null, Subject.StringPointId)] object? pointId)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(bool))!;

    // --arrange
    var unitId = new UnitId(parameterInfo, SpecialTag.Argument);
    var target = new IsParameterMarkedWithAttribute(pointId);

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_if_tag_is_not_argument([Values(null, "tag")] object? tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(string))!;

    // --arrange
    var unitId = new UnitId(parameterInfo, tag);
    var target = new IsParameterMarkedWithAttribute(Subject.StringPointId);

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_be_equal_if_point_id_equals([Values(null, "pointId")] object? pointId)
  {
    // --arrange
    var target1 = new IsParameterMarkedWithAttribute(pointId);
    var target2 = new IsParameterMarkedWithAttribute(pointId);

    //--assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal_if_point_id_differs([Values(null, "pointId")] object? pointId)
  {
    // --arrange
    var target1 = new IsParameterMarkedWithAttribute(pointId);
    var target2 = new IsParameterMarkedWithAttribute("different point id");

    //--assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public const string StringPointId = "pid";

    public static void Foo([Inject] int i, [Inject(StringPointId)] string s, bool b) { }
  }
}