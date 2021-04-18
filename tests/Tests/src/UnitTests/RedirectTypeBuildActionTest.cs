using Armature.Core;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;

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
      A.CallTo(() => buildSession.BuildSequence).Returns(new UnitId(null, SpecialKey.Propagate).AsArray());

      var buildAction = new RedirectType(typeof(int), expectedKey);

      // --act
      buildAction.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(Unit.OfType<int>(expectedKey))).MustHaveHappenedOnceExactly();
    }
  }
}
