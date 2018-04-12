using Armature.Core;
using Armature.Core.BuildActions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.UnitTests
{
  public class RedirectTypeBuildActionTest
  {
    [Test]
    public void should_propagate_token()
    {
      const string expectedToken = "token";
      // --arrange
      var buildSession = MockRepository.GenerateStub<IBuildSession>();
      buildSession.Stub(_ => _.BuildSequence).Return(new[] {new UnitInfo(null, Token.Propagate)});

      var buildAction = new RedirectTypeBuildAction(typeof(int), expectedToken);
      
      // --act
      buildAction.Process(buildSession);
      
      // --assert
      buildSession.AssertWasCalled(_ => _.BuildUnit(new UnitInfo(typeof(int), expectedToken)));
    }
  }
}