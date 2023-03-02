using Armature.BuildActions.Caching;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests.BuildActions;

public class SingletonTest
{
  [Test]
  public void should_not_set_build_result_if_empty()
  {
    // --arrange
    var target = new Singleton();
    var actual = new BuildSessionMock();

    // --act
    target.Process(actual);

    // --assert
    actual.BuildResult.HasValue.Should().BeFalse();
  }

  [Test]
  public void should_store_build_unit([Values("expected", null)] string expected)
  {
    // --arrange
    var target = new Singleton();

    // --act
    target.PostProcess(new BuildSessionMock(new BuildResult(expected))); // call where unit was built

    // --assert
    var actual = new BuildSessionMock();
    target.Process(actual); // next call
    actual.BuildResult.Value.Should().Be(expected);
  }
}