using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class GetConstructorByParameterTypesTest
{
  [Test]
  public void should_find_constructor_without_parameters()
  {
    // --arrange
    var target = new GetConstructorByParameterTypes();

    // --act
    var actual = new BuildSessionMock(Unit.IsType<Subject>().ToBuildSequence());
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
    var actual = new BuildSessionMock(Unit.IsType<Subject>().ToBuildSequence());
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
    var actual = new BuildSessionMock(Unit.IsType<Subject>().ToBuildSequence());
    target.Process(actual);

    // --assert
    actual.BuildResult.HasValue.Should().BeFalse();
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public Subject() { }
    public Subject(int i) { }
    public Subject(int i, string? s = null) { }
    public Subject(short i, string? s = null) { }
  }
}