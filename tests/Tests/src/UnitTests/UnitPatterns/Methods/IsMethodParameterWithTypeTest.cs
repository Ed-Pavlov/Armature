using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.UnitPatterns.Methods;

public class IsMethodParameterWithTypeTest
{
  [Test]
  public void should_delegate_call_with_extracted_type()
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;
    var expected      = new UnitId(typeof(int), null);

    // --arrange
    var typePattern = A.Fake<IUnitPattern>();
    var target      = new IsMethodParameterWithType(typePattern);
    var unitId      = new UnitId(parameterInfo, SpecialKey.Argument);

    // --act
    target.Matches(unitId);

    // --assert
    A.CallTo(() => typePattern.Matches(expected)).MustHaveHappenedOnceAndOnly();
  }

  [Test]
  public void should_match_only_argument_special_key([Values(null, "key")] object? key)
  {
    var parameterInfo = typeof(Subject).GetMethod(nameof(Subject.Foo))?.GetParameters().Single(_ => _.ParameterType == typeof(int))!;

    // --arrange
    var typePattern = A.Fake<IUnitPattern>();
    var target      = new IsMethodParameterWithType(typePattern);
    var unitId      = new UnitId(parameterInfo, key);

    // --act
    target.Matches(unitId);

    // --assert
    A.CallTo(() => typePattern.Matches(An<UnitId>._)).MustNotHaveHappened();
  }

  [Test]
  public void should_check_argument_for_null()
  {
    // --arrange
    var target = () => new IsMethodParameterWithType(null!);

    // --act
    // --assert
    target.Should().ThrowExactly<ArgumentNullException>("typePattern");
  }

  [Test]
  public void should_be_equal_if_pattern_equal()
  {
    // --arrange
    var target1 = new IsMethodParameterWithType(new UnitPattern("1"));
    var target2 = new IsMethodParameterWithType(new UnitPattern("1"));

    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [Test]
  public void should_not_be_equal_if_pattern_differs()
  {
    // --arrange
    var target1 = new IsMethodParameterWithType(new UnitPattern("1"));
    var target2 = new IsMethodParameterWithType(new UnitPattern("2"));

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