using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests.BuildActions;

public class GetParameterDefaultValueTest
{
  [Test]
  public void should_get_default_value()
  {
    var parameter = typeof(Subject).GetMethod(nameof(Subject.Foo))!.GetParameters().Single(_ => _.HasDefaultValue);

    // --arrange
    var buildSession = new BuildSessionMock(Unit.Is(parameter).Key(SpecialKey.Argument).ToBuildChain());
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
    var buildSession = new BuildSessionMock(Unit.Is(parameter).Key(SpecialKey.Argument).ToBuildChain());
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