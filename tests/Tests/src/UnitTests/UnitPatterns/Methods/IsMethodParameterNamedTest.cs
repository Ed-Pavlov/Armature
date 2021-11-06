using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsMethodParameterNamedTest
{
  [Test]
  public void should_match_parameter_by_name()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "i")!;

    // --arrange
    var unitId = new UnitId(parameterInfo, SpecialKey.Argument);
    var target = new IsMethodParameterNamed(parameterInfo.Name!);

    // --act
    // --assert
    target.Matches(unitId).Should().BeTrue();
  }

  [Test]
  public void should_not_match_parameter_if_name_differs()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "i")!;

    // --arrange
    var unitId = new UnitId(parameterInfo, SpecialKey.Argument);
    var target = new IsMethodParameterNamed("another parameter name");

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_not_match_if_key_is_not_argument([Values(null, "key")] object key)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.Name == "i")!;

    // --arrange
    var unitId = new UnitId(parameterInfo, key);
    var target = new IsMethodParameterNamed(parameterInfo.Name!);

    // --act
    // --assert
    target.Matches(unitId).Should().BeFalse();
  }

  [Test]
  public void should_be_equal_if_method_name_equals()
  {
    const string method = "method";

    // --arrange
    var target1 = new IsMethodParameterNamed(method);
    var target2 = new IsMethodParameterNamed(method);

    // --act
    // --assert
    target1.Equals(target2).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal_if_method_name_differs()
  {
    // --arrange
    var target1 = new IsMethodParameterNamed("method1");
    var target2 = new IsMethodParameterNamed("method2");

    // --act
    // --assert
    target1.Equals(target2).Should().BeFalse();
  }

  [Test]
  public void should_not_allow_null_argument([Values(null, "")] string? methodName)
  {
    // --arrange
    var actual = () => new IsMethodParameterNamed(methodName);

    // --act
    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("name");
  }

  [Test]
  public void should_be_equal_if_name_equal()
  {
    // --arrange
    var target1 = new IsMethodParameterNamed("methodName");
    var target2 = new IsMethodParameterNamed("methodName");

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal_if_pattern_differs()
  {
    // --arrange
    var target1 = new IsMethodParameterNamed("methodName1");
    var target2 = new IsMethodParameterNamed("methodName2");

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