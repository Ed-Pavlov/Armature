using System.Linq;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.BuildActions;

public class GetParameterDefaultValueTest
{
  [Test]
  public void should_get_default_value()
  {
    var parameter = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.HasDefaultValue);

    // --arrange
    var buildSession = new BuildSessionMock(Unit.By(parameter, ServiceTag.Argument).ToBuildStack());
    var target       = new GetParameterDefaultValue();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(Subject.Expected);
  }

  [Test]
  public void should_do_nothing_if_no_default_value()
  {
    var parameter = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => !_.HasDefaultValue);

    // --arrange
    var buildSession = new BuildSessionMock(Unit.By(parameter, ServiceTag.Argument).ToBuildStack());
    var target       = new GetParameterDefaultValue();

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.HasValue.Should().BeFalse();
  }

  public class Subject
  {
    public const  string Expected = nameof(Expected);
    public static void   Foo(int i, string s = Expected){}
  }
}