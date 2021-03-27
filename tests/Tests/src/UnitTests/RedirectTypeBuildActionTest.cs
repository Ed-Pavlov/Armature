using Armature.Core;
using Armature.Core.BuildActions;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests
{
  public class RedirectTypeBuildActionTest
  {
    [Test]
    public void should_propagate_token()
    {
      const string expectedToken = "token";
      
      // --arrange
      var buildSession = A.Fake<IBuildSession>();
      
      A.CallTo(() => buildSession.BuildSequence).Returns(new UnitInfo(null, Token.Propagate).AsArray());

      var target = new RedirectTypeBuildAction(typeof(int), expectedToken);

      // --act
      target.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(Unit.OfType<int>(expectedToken))).MustHaveHappenedOnceExactly();
    }
  }
}