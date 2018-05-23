using System;
using System.IO;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.UnitTests
{
  public class CreateByReflectionBuildActionTest
  {
    [Test]
    public void should_not_create_interface_type()
    {
      // --arrange
      var target = CreateByReflectionBuildAction.Instance;

      var buildSession = MockRepository.GenerateStub<IBuildSession>();
      buildSession.Stub(_ => _.BuildSequence).Return(new[] {new UnitInfo(typeof(IDisposable), null)});

      // --act
      target.Process(buildSession);

      // --assert
      buildSession.AssertWasNotCalled(_ => _.BuildUnit(null), _ => _.IgnoreArguments());
    }

    [Test]
    public void should_not_create_abstract_type()
    {
      // --arrange
      var target = CreateByReflectionBuildAction.Instance;

      var buildSession = MockRepository.GenerateStub<IBuildSession>();
      buildSession.Stub(_ => _.BuildSequence).Return(new[] {new UnitInfo(typeof(Stream), null)});

      // --act
      target.Process(buildSession);

      // --assert
      buildSession.AssertWasNotCalled(_ => _.BuildUnit(null), _ => _.IgnoreArguments());
    }
  }
}