using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class GetConstructorByInjectPointIdTest
{
  [Test]
  public void should_find_constructor_with_point_id_among_others()
  {
    // --arrange
    var target = new GetConstructorByInjectPoint(Subject.IntId);

    // --act
    var actual = new BuildSessionMock(Kind.Is<Subject>().ToBuildChain());
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.As<ConstructorInfo>().GetParameters().Single().ParameterType.Should().Be<int>();
  }

  [Test]
  public void should_find_marked_constructor_without_point_id_among_others()
  {
    // --arrange
    var target = new GetConstructorByInjectPoint();

    // --act
    var actual = new BuildSessionMock(Kind.Is<Subject>().ToBuildChain());
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.As<ConstructorInfo>().GetParameters().Single().ParameterType.Should().Be<byte>();
  }

  [Test]
  public void should_return_no_constructor()
  {
    // --arrange
    var target = new GetConstructorByInjectPoint("bad-id");

    // --act
    var actual = new BuildSessionMock(Kind.Is<Subject>().ToBuildChain());
    target.Process(actual);

    // --assert
    actual.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_fail_if_more_than_one_constructor_matched()
  {
    // --arrange
    var target = new GetConstructorByInjectPoint(Subject.AmbiguousId);

    // --act
    Action actual = () => target.Process(new BuildSessionMock(Kind.Is<Subject>().ToBuildChain()));

    // --assert
    actual.Should().ThrowExactly<ArmatureException>().Which.Message.Should().StartWith("More than one constructors of the type");
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
  private class Subject
  {
    public const string AmbiguousId = nameof(AmbiguousId);
    public const string IntId = nameof(IntId);

    [Inject]
    public Subject(byte  b) { }
    [Inject(IntId)]
    public Subject(int i) { }
    [Inject(AmbiguousId)]
    public Subject(short s) { }
    [Inject(AmbiguousId)]
    public Subject(long  l) { }
  }
}