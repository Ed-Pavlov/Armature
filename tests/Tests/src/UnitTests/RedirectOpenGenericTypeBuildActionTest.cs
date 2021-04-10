using System.Collections.Generic;
using Armature.Core;
using Armature.Core.BuildActions;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests
{
  public class RedirectOpenGenericTypeBuildActionTest
  {
    [Test]
    public void should_propagate_key()
    {
      const string expectedKey = "key";

      // --arrange
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.OfType<IEnumerable<int>>(UnitKey.Propagate).AsArray());

      var buildAction = new RedirectOpenGenericTypeBuildAction(typeof(List<>), expectedKey);

      // --act
      buildAction.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(Unit.OfType<List<int>>(expectedKey))).MustHaveHappenedOnceExactly();

      // buildSession.AssertWasCalled(_ => _.BuildUnit(new UnitInfo(typeof(List<int>), expectedKey)));
    }
  }
}
