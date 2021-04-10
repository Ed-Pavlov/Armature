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
    public void should_propagate_token()
    {
      const string expectedToken = "token";

      // --arrange
      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.OfType<IEnumerable<int>>(UnitKey.Propagate).AsArray());

      var buildAction = new RedirectOpenGenericTypeBuildAction(typeof(List<>), expectedToken);

      // --act
      buildAction.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(Unit.OfType<List<int>>(expectedToken))).MustHaveHappenedOnceExactly();

      // buildSession.AssertWasCalled(_ => _.BuildUnit(new UnitInfo(typeof(List<int>), expectedToken)));
    }
  }
}
