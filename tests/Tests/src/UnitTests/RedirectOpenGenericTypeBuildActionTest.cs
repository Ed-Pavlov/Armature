using System.Collections.Generic;
using Armature.Core;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;
using Tests.UnitTests.BuildActions;

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
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.IsType<IEnumerable<int>>().Key(SpecialKey.Propagate).ToBuildSequence());

      var buildAction = new RedirectOpenGenericType(typeof(List<>), expectedKey);

      // --act
      buildAction.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(Unit.IsType<List<int>>().Key(expectedKey))).MustHaveHappenedOnceExactly();

      // buildSession.AssertWasCalled(_ => _.BuildUnit(new UnitInfo(typeof(List<int>), expectedKey)));
    }
  }
}