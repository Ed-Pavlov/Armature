using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class GetConstructorByParameterTypesTest
{
  [Test]
  public void should_find_constructor_without_parameters()
  {
    // --arrange
    var target = new GetConstructorByParameterTypes();

    // --act
    var actual = new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack());
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.As<ConstructorInfo>().GetParameters().Should().HaveCount(0);
  }


  [Test]
  public void should_find_constructor_with_optional_parameters()
  {
    var expectedTypes = new[] {typeof(int), typeof(string)};
    // --arrange
    var target = new GetConstructorByParameterTypes(expectedTypes);

    // --act
    var actual = new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack());
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.As<ConstructorInfo>().GetParameters().Select(_ => _.ParameterType).Should().Equal(expectedTypes);
  }

  [Test]
  public void should_return_no_constructor()
  {
    // --arrange
    var target = new GetConstructorByParameterTypes(typeof(short));

    // --act
    var actual = new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack());
    target.Process(actual);

    // --assert
    actual.BuildResult.HasValue.Should().BeFalse();
  }

  private static IEnumerable<TestCaseData> should_be_equal_source()
  {
    yield return new TestCaseData(new GetConstructorByParameterTypes(), new GetConstructorByParameterTypes());

    yield return new TestCaseData(
        new GetConstructorByParameterTypes(typeof(string), typeof(int)),
        new GetConstructorByParameterTypes(typeof(string), typeof(int)));

    var referenceEqual = new GetConstructorByParameterTypes(typeof(string));
    yield return new TestCaseData(referenceEqual, referenceEqual);
  }

  [TestCaseSource(nameof(should_be_equal_source))]
  public void should_be_equal(GetConstructorByParameterTypes target1, GetConstructorByParameterTypes target2)
  {
    // --assert
    target1.Equals(target2).Should().BeTrue();
    target2.Equals(target1).Should().BeTrue();
  }

  [TestCaseSource(nameof(types_for_not_equality))]
  public void should_not_be_equal(Type[] types1, Type[] types2)
  {
    // --arrange
    var target1 = new GetConstructorByParameterTypes(types1);
    var target2 = new GetConstructorByParameterTypes(types2);

    // --assert
    target1.Equals(target2).Should().BeFalse();
    target2.Equals(target1).Should().BeFalse();
  }

  [Test]
  public void should_not_be_equal_to_null()
  {
    // --arrange
    var target = new GetConstructorByParameterTypes();

    // --assert
    target.Equals(null).Should().BeFalse();
  }

  [TestCaseSource(nameof(null_types))]
  public void should_check_types_for_null(Type[] types)
  {
    // --arrange
    var actual = () => new GetConstructorByParameterTypes(types);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("parameterTypes");
  }

  private static IEnumerable<TestCaseData> types_for_not_equality()
  {
    yield return new TestCaseData(new[] {typeof(string), typeof(int)}, new[] {typeof(int), typeof(string)});
    yield return new TestCaseData(new[] {typeof(int), typeof(int)}, new[] {typeof(int)});
  }

  private static IEnumerable<Type[]> null_types()
  {
    yield return null!;
    yield return new[] {typeof(string), null!, typeof(int)};
  }

  [UsedImplicitly]
  private class Subject
  {
    public Subject() { }
    public Subject(int i) { }
    public Subject(int i, string? s = null) { }
    public Subject(short i, string? s = null) { }
  }
}