﻿using System;
using System.IO;
using Armature.Core;
using Armature.Core.BuildActions.Creation;
using FakeItEasy;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests
{
  public class CreateByReflectionBuildActionTest
  {
    [Test]
    public void should_not_create_interface_type()
    {
      // --arrange
      var target = CreateByReflectionBuildAction.Instance;

      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.OfType<IDisposable>().AsArray());

      // --act
      target.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(null)).WithAnyArguments().MustNotHaveHappened();
    }

    [Test]
    public void should_not_create_abstract_type()
    {
      // --arrange
      var target = CreateByReflectionBuildAction.Instance;

      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.OfType<Stream>().AsArray());

      // --act
      target.Process(buildSession);

      // --assert
      A.CallTo(() => buildSession.BuildUnit(null)).WithAnyArguments().MustNotHaveHappened();
    }
  }
}
