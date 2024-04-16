using System;
using Armature;
using Armature.Core;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;
using Void = Tests.Util.Void;

namespace Tests.UnitTests.BuildActions.Creation;

public class CreateWithFactoryMethod_NTest
{
  [Test]
  public void N1_test()
  {
    const int    expectedArgument = 3987;
    const string expectedResult   = "expected";

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, Void, Void, Void, Void, Void, Void>();
    buildArguments.Returns(expectedArgument.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, string>(arg => arg == expectedArgument ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }

  public void N2_test()
  {
    const string expectedResult    = "expected";
    object?[]    expectedArguments = {1, 2};

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, int>();
    buildArguments.Returns(expectedArguments.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, int, string>(
        (_1, _2) => _1 == 1 && _2 == 2 ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }

  [Test]
  public void N3_test()
  {
    const string expectedResult    = "expected";
    object?[]    expectedArguments = {1, 2, 3};

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, int, int>();
    buildArguments.Returns(expectedArguments.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, int, int, string>(
        (_1, _2, _3) => _1 == 1 && _2 == 2  && _3 == 3 ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }

  [Test]
  public void N4_test()
  {
    const string expectedResult    = "expected";
    object?[]    expectedArguments = {1, 2, 3, 4};

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, int, int, int>();
    buildArguments.Returns(expectedArguments.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, int, int, int, string>(
        (_1, _2, _3, _4) => _1 == 1 && _2 == 2  && _3 == 3  && _4 == 4 ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }

  [Test]
  public void N5_test()
  {
    const string expectedResult    = "expected";
    object?[]    expectedArguments = {1, 2, 3, 4, 5};

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, int, int, int, int>();
    buildArguments.Returns(expectedArguments.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, int, int, int, int, string>(
        (_1, _2, _3, _4, _5) => _1 == 1 && _2 == 2  && _3 == 3  && _4 == 4  && _5 == 5 ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }

  [Test]
  public void N6_test()
  {
    const string expectedResult    = "expected";
    object?[]    expectedArguments = {1, 2, 3, 4, 5, 6};

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, int, int, int, int, int>();
    buildArguments.Returns(expectedArguments.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, int, int, int, int, int, string>(
        (_1, _2, _3, _4, _5, _6) => _1 == 1 && _2 == 2  && _3 == 3  && _4 == 4  && _5 == 5  && _6 == 6 ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }

  [Test]
  public void N7_test()
  {
    const string expectedResult    = "expected";
    object?[]    expectedArguments = {1, 2, 3, 4, 5, 6, 7};

    // --arrange
    var buildSession   = A.Fake<IBuildSession>();
    var buildArguments = A.CallTo(() => buildSession.BuildUnit(default, true)).WhenBuildArgumentsOfType<int, int, int, int, int, int, int>();
    buildArguments.Returns(expectedArguments.ToArguments());

    var target = new CreateWithFactoryMethodBuildAction<int, int, int, int, int, int, int, string>(
        (_1, _2, _3, _4, _5, _6, _7) => _1 == 1 && _2 == 2  && _3 == 3  && _4 == 4  && _5 == 5  && _6 == 6  && _7 == 7 ? expectedResult : throw new Exception());

    // --act
    target.Process(buildSession);

    // --assert
    buildArguments.MustHaveHappenedOnceExactly();
    buildSession.BuildResult.Value.Should().Be(expectedResult);
  }
}