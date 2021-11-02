using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.BuildActions;

public class InstanceTest
{
  [Test]
  public void process_should_return_instance([Values("expected", null)] string expected)
  {
    // --arrange
    var buildSession = new BuildSessionMock();
    var target       = new Instance<string>(expected);

    // --act
    target.Process(buildSession);

    // --assert
    buildSession.BuildResult.Value.Should().Be(expected);
  }
}