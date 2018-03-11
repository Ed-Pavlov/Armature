using System.Collections.Generic;
using Armature.Core;
using Armature.Framework.BuildActions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.UnitTests
{
  public class RedirectOpenGenericTypeBuildActionTest
  {
    [Test]
    public void should_propagate_token()
    {
      const string expectedToken = "token";
      // --arrange
      var buildSession = MockRepository.GenerateStub<IBuildSession>();
      buildSession.Stub(_ => _.BuildSequence).Return(new[] {new UnitInfo(typeof(IEnumerable<int>), Token.Propagate)});

      var buildAction = new RedirectOpenGenericTypeBuildAction(typeof(List<>), expectedToken);
      
      // --act
      buildAction.Process(buildSession);
      
      // --assert
      buildSession.AssertWasCalled(_ => _.BuildUnit(new UnitInfo(typeof(List<int>), expectedToken)));
    }
  }
}