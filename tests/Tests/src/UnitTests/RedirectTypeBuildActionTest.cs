using Armature.Core;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;
using Tests.UnitTests.BuildActions;
using TestUtils = Tests.UnitTests.BuildActions.TestUtils;

namespace Tests.UnitTests
{
  public class RedirectTypeBuildActionTest
  {
    [Test]
    public void should_propagate_key()
    {
      const string expectedKey = "key";

      // --arrange
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.Is(null).Key(SpecialKey.Propagate).ToBuildSequence());

      var buildAction = new RedirectType(typeof(int), expectedKey);

      // --act
      buildAction.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(Unit.IsType<int>().Key(expectedKey))).MustHaveHappenedOnceExactly();
    }
  }
}