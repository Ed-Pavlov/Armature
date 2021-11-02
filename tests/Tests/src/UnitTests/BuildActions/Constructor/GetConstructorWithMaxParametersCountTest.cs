using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.BuildActions;

public class GetConstructorWithMaxParametersCountTest
{
  [Test]
  public void should_return_constructor()
  {
    // --arrange
    var target = new GetConstructorWithMaxParametersCount();
    var actual = new BuildSessionMock(new UnitId(typeof(Good), null));

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.As<ConstructorInfo>().GetParameters().Select(_ => _.ParameterType).Should().Equal(typeof(int), typeof(string), typeof(bool));
  }

  [Test]
  public void should_fail_if_more_than_one_constructor_matched()
  {
    // --arrange
    var target = new GetConstructorWithMaxParametersCount();

    // --act
    Action actual = () => target.Process(new BuildSessionMock(new UnitId(typeof(Bad), null)));

    // --assert
    actual.Should().ThrowExactly<ArmatureException>().Which.Message.Should().StartWith("More than one constructor with max parameters count for type");
  }

  [Test]
  public void should_return_no_constructor()
  {
    // --arrange
    var target = new GetConstructorWithMaxParametersCount();
    var actual = new BuildSessionMock(new UnitId(typeof(NoCtor), null));

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.HasValue.Should().BeFalse();
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Good
  {
    public Good() { }
    public Good(int i) { }
    public Good(int i, string s) { }
    public Good(int i = 38, string s = "", bool b = false) { }
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Bad
  {
    public Bad() { }
    public Bad(int    i) { }
    public Bad(int    i, string s) { }
    public Bad(string s, int    i) { }
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private class NoCtor
  {
    private NoCtor() { }
  }
}