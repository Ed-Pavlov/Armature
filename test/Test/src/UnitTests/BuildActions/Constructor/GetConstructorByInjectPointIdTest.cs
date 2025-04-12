using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Armature.Test.Util;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.UnitTests.BuildActions;

public class GetConstructorByInjectPointIdTest
{
  [Test]
  public void should_find_constructor_with_point_id_among_others()
  {
    // --arrange
    var target = new GetConstructorByInjectPoint(Subject.IntId);

    // --act
    var actual = new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack());
    target.Process(actual);

    // --assert
    actual.BuildResult.Value.As<ConstructorInfo>().GetParameters().Single().ParameterType.Should().Be<int>();
  }

  [Test]
  public void should_find_marked_constructor_without_point_id_among_others()
  {
    // --arrange
    var target = new GetConstructorByInjectPoint(null);

    // --act
    var actual = new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack());
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
    var actual = new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack());
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
    Action actual = () => target.Process(new BuildSessionMock(TUnit.OfType<Subject>().ToBuildStack()));

    // --assert
    actual.Should().ThrowExactly<ArmatureException>().Which.Message.Should().StartWith("More than one constructors of the type");
  }

  [Test]
  public void should_find_any_constructor_by_set_of_tags_with_any()
  {
    // --arrange
    var tags   = new object?[] {"bad-id", null, Tag.Any};
    var target = new GetConstructorByInjectPoint(BindingFlags.Instance | BindingFlags.Public, tags);

    // --act
    AssertFoundTypeIs<AnotherSubject>(target);
  }

  [Test]
  public void should_find_constructors_by_set_of_tags()
  {
    // --arrange
    var tags   = new object?[] {OneAnotherSubject.OneAnotherId, AnotherSubject.AnotherId, null};
    var target = new GetConstructorByInjectPoint(BindingFlags.Instance | BindingFlags.Public, tags);

    // --act
    AssertFoundTypeIs<Subject>(target);
    AssertFoundTypeIs<AnotherSubject>(target);
    AssertFoundTypeIs<OneAnotherSubject>(target);
  }

  private static void AssertFoundTypeIs<TSubject>(GetConstructorByInjectPoint target)
  {
    // --act
    // imitating we build Unit of type TSubject
    var buildSession = new BuildSessionMock(TUnit.OfType<TSubject>().ToBuildStack());
    target.Process(buildSession); // should find the corresponding constructor of TSubject

    // --assert
    buildSession.BuildResult.Value.As<ConstructorInfo>().Should().NotBeNull();
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
  private class Subject
  {
    public const string AmbiguousId = nameof(AmbiguousId);
    public const string IntId       = nameof(IntId);

    [Inject]
    public Subject(byte b) { }
    [Inject(IntId)]
    public Subject(int i) { }
    [Inject(AmbiguousId)]
    public Subject(short s) { }
    [Inject(AmbiguousId)]
    public Subject(long l) { }
  }

  private class AnotherSubject
  {
    public const string AnotherId = nameof(AnotherId);

    [Inject(AnotherId)]
    public AnotherSubject() { }
  }

  private class OneAnotherSubject
  {
    public const string OneAnotherId = nameof(OneAnotherId);

    [Inject(OneAnotherId)]
    public OneAnotherSubject() { }
  }
}
