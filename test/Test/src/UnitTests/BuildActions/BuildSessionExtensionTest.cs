using System;
using System.Reflection;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.UnitTests.BuildActions;

public class BuildSessionExtensionTest
{
  [Test]
  public void GetConstructorOf_should_throw_if_buildSession_is_null()
  {
    // --arrange
    var action = () => BuildSessionExtension.GetConstructorOf(null!, typeof(string));

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("buildSession");
  }

  [Test]
  public void GetConstructorOf_should_throw_if_type_is_null()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var action       = () => buildSession.GetConstructorOf(null!);

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("type");
  }

  [Test]
  public void GetConstructorOf_should_throw_if_no_value_produced()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(A<UnitId>._)).Returns(default(BuildResult)); // Simulate that nothing is produced

    var action = () => buildSession.GetConstructorOf(typeof(string));

    // --assert
    action.Should().ThrowExactly<ArmatureException>();
  }

  [Test]
  public void GetConstructorOf_should_return_produced_value()
  {
    // --arrange
    var expected     = typeof(string).GetConstructors()[0];
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(typeof(string), ServiceTag.Constructor))).Returns(new BuildResult(expected));

    // --act
    var actual = buildSession.GetConstructorOf(typeof(string));

    // --assert
    actual.Should().BeSameAs(expected);
  }

  [Test]
  public void BuildArgumentsForMethod_should_throw_if_buildSession_is_null()
  {
    // --arrange
    var parameters = typeof(string).GetConstructors()[0].GetParameters();
    var action     = () => BuildSessionExtension.BuildArgumentsForMethod(null!, parameters);

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("buildSession");
  }

  [Test]
  public void BuildArgumentsForMethod_should_throw_if_parameters_is_null()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var action       = () => buildSession.BuildArgumentsForMethod(null!);

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("parameters");
  }

  [Test]
  public void BuildArgumentsForMethod_should_throw_if_parameters_is_empty()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var action       = () => buildSession.BuildArgumentsForMethod(Array.Empty<ParameterInfo>());

    // --assert
    action.Should().ThrowExactly<ArgumentException>().WithParameterName("parameters");
  }

  [Test]
  public void BuildArgumentsForMethod_should_throw_if_no_value_produced()
  {
    // --arrange
    var parameters   = typeof(string).GetConstructors()[0].GetParameters();
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(parameters, ServiceTag.Argument))).Returns(default(BuildResult)); // Simulate that nothing is produced

    var action = () => buildSession.BuildArgumentsForMethod(parameters);

    // --assert
    action.Should().ThrowExactly<ArmatureException>();
  }

  [Test]
  public void BuildArgumentsForMethod_should_throw_if_wrong_arguments_count()
  {
    // --arrange
    var parameters   = typeof(Tuple<int, string>).GetConstructors()[0].GetParameters(); // requires two arguments
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(parameters, ServiceTag.Argument)))
     .Returns(new BuildResult(new object[1])); // but BuildUnit returns only one

    // --act
    var action = () => buildSession.BuildArgumentsForMethod(parameters);

    // --assert
    action.Should().ThrowExactly<ArmatureException>();
  }

  [Test]
  public void BuildArgumentsForMethod_should_return_produced_value()
  {
    // --arrange
    var parameters   = typeof(string).GetConstructors()[0].GetParameters();
    var expected     = new object[parameters.Length];
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(parameters, ServiceTag.Argument))).Returns(new BuildResult(expected));

    // --act
    var actual = buildSession.BuildArgumentsForMethod(parameters);

    // --assert
    actual.Should().BeSameAs(expected);
  }

  [Test]
  public void BuildArgumentForMethod_should_throw_if_buildSession_is_null()
  {
    // --arrange
    var parameter = typeof(string).GetConstructors()[0].GetParameters()[0];
    var action    = () => BuildSessionExtension.BuildArgumentForMethod(null!, parameter);

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("buildSession");
  }

  [Test]
  public void BuildArgumentForMethod_should_throw_if_parameter_is_null()
  {
    // --arrange
    var buildSession = A.Fake<IBuildSession>();
    var action       = () => buildSession.BuildArgumentForMethod(null!);

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("parameter");
  }

  [Test]
  public void BuildArgumentForMethod_should_throw_if_no_value_produced()
  {
    // --arrange
    var parameter    = typeof(string).GetConstructors()[0].GetParameters()[0];
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(parameter, ServiceTag.Argument))).Returns(default(BuildResult)); // Simulate that nothing is produced

    var action = () => buildSession.BuildArgumentForMethod(parameter);

    // --assert
    action.Should().ThrowExactly<ArmatureException>();
  }

  [Test]
  public void BuildArgumentForMethod_should_return_produced_value()
  {
    // --arrange
    var parameter    = typeof(string).GetConstructors()[0].GetParameters()[0];
    var expected     = new object();
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(parameter, ServiceTag.Argument))).Returns(new BuildResult(expected));

    // --act
    var actual = buildSession.BuildArgumentForMethod(parameter);

    // --assert
    actual.Should().BeSameAs(expected);
  }

  [Test]
  public void BuildPropertyArgument_should_throw_if_buildSession_is_null()
  {
    // --arrange
    var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property));
    var action       = () => BuildSessionExtension.BuildPropertyArgument(null!, propertyInfo!);

    // --assert
    action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("buildSession");
  }

  [Test]
  public void BuildPropertyArgument_should_throw_if_no_value_produced()
  {
    // --arrange
    var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property));
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(propertyInfo, ServiceTag.Argument))).Returns(default(BuildResult)); // Simulate that nothing is produced

    var action = () => buildSession.BuildPropertyArgument(propertyInfo!);

    // --assert
    action.Should().ThrowExactly<ArmatureException>();
  }

  [Test]
  public void BuildPropertyArgument_should_return_produced_value()
  {
    // --arrange
    var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.Property));
    var expected     = new object();
    var buildSession = A.Fake<IBuildSession>();
    A.CallTo(() => buildSession.BuildUnit(new UnitId(propertyInfo, ServiceTag.Argument))).Returns(new BuildResult(expected));

    // --act
    var actual = buildSession.BuildPropertyArgument(propertyInfo!);

    // --assert
    actual.Should().BeSameAs(expected);
  }

  private class TestClass
  {
    public object? Property { get; set; }
  }
}
