﻿using System;
using System.IO;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace Tests.UnitTests
{
  public class GetConstructorByParameterTypesBuildActionTest
  {
    [Test]
    public void should_find_constructor()
    {
      var parameterTypes = new []{typeof(int), typeof(IDisposable)};
      
      // --arrange
      var target = new GetConstructorByParameterTypesBuildAction(parameterTypes);

      var buildSession = MockRepository.GenerateStub<IBuildSession>();
      buildSession.Stub(_ => _.BuildSequence).Return(new[] {new UnitInfo(typeof(SampleType), SpecialToken.Constructor)});
      
      // --act
      target.Process(buildSession);
      
      // --assert
      buildSession.BuildResult.Should().NotBeNull();
      buildSession.BuildResult.Value.Should().Be(typeof(SampleType).GetConstructor(parameterTypes));
    }

    [Test]
    public void should_not_set_build_result_if_ctor_is_not_found()
    {
      var parameterTypes = new []{typeof(int), typeof(MemoryStream)};
      
      // --arrange
      var target = new GetConstructorByParameterTypesBuildAction(parameterTypes);

      var buildSession = MockRepository.GenerateStub<IBuildSession>();
      buildSession.Stub(_ => _.BuildSequence).Return(new[] {new UnitInfo(typeof(SampleType), SpecialToken.Constructor)});
      
      // --act
      target.Process(buildSession);
      
      // --assert
      buildSession.BuildResult.Should().BeNull();
    }

    private class SampleType
    {
      public SampleType(int i, IDisposable d)
      {
      }
    }
  }
}