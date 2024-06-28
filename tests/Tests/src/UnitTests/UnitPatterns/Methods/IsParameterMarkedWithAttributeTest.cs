using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
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
    var unitId = Unit.By(parameterInfo, ServiceTag.Argument);
    var target = new IsParameterAttributed();

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_parameter_with_point_id()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(string))!;

    // --arrange
    var unitId = Unit.By(parameterInfo, ServiceTag.Argument);
    var target = new IsParameterAttributed(Subject.StringPointId);

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_match_parameter_without_point_id([Values(null, Subject.StringPointId)] object? pointId)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(bool))!;

    // --arrange
    var unitId = Unit.By(parameterInfo, ServiceTag.Argument);
    var target = new IsParameterAttributed(pointId);

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_if_tag_is_not_argument([Values(null, "tag")] object? tag)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(string))!;

    // --arrange
    var unitId = Unit.By(parameterInfo, tag);
    var target = new IsParameterAttributed(Subject.StringPointId);

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_be_equal_if_point_id_equals([Values(null, "pointId")] object? pointId)
  {
    // --arrange
    var target1 = new IsParameterAttributed(pointId);
    var target2 = new IsParameterAttributed(pointId);

    //--assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal_if_point_id_differs([Values(null, "pointId")] object? pointId)
  {
    // --arrange
    var target1 = new IsParameterAttributed(pointId);
    var target2 = new IsParameterAttributed("different point id");

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