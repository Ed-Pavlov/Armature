using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests
{
  public class GetConstructorByParameterTypesBuildActionTest
  {
    [Test]
    public void should_find_constructor()
    {
      var parameterTypes = new[] {typeof(int), typeof(IDisposable)};

      // --arrange
      var target = new GetConstructorByParameterTypesBuildAction(parameterTypes);

      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.OfType<SampleType>(SpecialToken.Constructor).AsArray());

      // --act
      target.Process(buildSession);

      // --assert
      buildSession.BuildResult.HasValue.Should().BeTrue();
      buildSession.BuildResult.Value.Should().Be(typeof(SampleType).GetConstructor(parameterTypes));
    }

    [Test]
    public void should_not_set_build_result_if_ctor_is_not_found()
    {
      var parameterTypes = new[] {typeof(int), typeof(MemoryStream)};

      // --arrange
      var target = new GetConstructorByParameterTypesBuildAction(parameterTypes);

      var buildSession = A.Fake<IBuildSession>();
      A.CallTo(() => buildSession.BuildSequence).Returns(Unit.OfType<SampleType>(SpecialToken.Constructor).AsArray());

      // --act
      target.Process(buildSession);

      // --assert
      buildSession.BuildResult.HasValue.Should().BeFalse();
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private class SampleType
    {
      public SampleType(int i, IDisposable d)
      {
      }
    }
  }
}