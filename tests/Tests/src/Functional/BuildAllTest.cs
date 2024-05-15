using System;
using System.Linq;
using Armature;
using Armature.Core;
using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.Functional
{
  public class BuildAllTest
  {
    [Test]
    public void should_return_two_implementation_of_one_interface()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposable>().AsCreated<SampleType1>();
      target.Treat<IDisposable>().AsCreated<SampleType2>();

      // --act
      var actual = target.BuildAll<IDisposable>();

      // --assert
      actual.Should().HaveCount(2);
      actual.Should().ContainSingle(_ => _ is SampleType1);
      actual.Should().ContainSingle(_ => _ is SampleType2);
    }

    [Test]
    public void should_throw_if_more_than_one_build_stage_involved()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposable>().AsCreated<SampleType1>();
      target.Treat<IDisposable>().AsCreated<SampleType2>();
      target.Treat<IDisposable>().AsSingleton(); // involve BuildStage.Cache for first level unit

      // --act
      Action actual = () => target.BuildAllUnits(TUnit.OfType<IDisposable>());

      // --assert
      actual.Should().ThrowExactly<ArmatureException>();
    }

    [Test]
    public void more_than_one_build_stage_can_be_used_for_redirected_type()
    {
      // --arrange
      var target = CreateTarget();

      target.Treat<IDisposable>().AsCreated<SampleType1>().AsSingleton();
      target.Treat<IDisposable>().AsCreated<SampleType2>();

      // --precondition
      var precondition = target.BuildAll<IDisposable>();
      precondition.Should().HaveCount(2);
      precondition.Should().ContainSingle(_ => _ is SampleType1);
      precondition.Should().ContainSingle(_ => _ is SampleType2);
      var expected = precondition.Single(_ => _ is SampleType1);

      // --act
      var actual = target.BuildAll<IDisposable>();
      actual.Should().HaveCount(2);
      actual.Should().ContainSingle(_ => _ is SampleType1);
      actual.Should().ContainSingle(_ => _ is SampleType2);
      actual.Single(_ => _ is SampleType1).Should().BeSameAs(expected);
      actual.Single(_ => _ is SampleType2).Should().NotBeSameAs(precondition.Single(_ => _ is SampleType2));
    }

    private static Builder CreateTarget()
      => new("test", BuildStage.Cache, BuildStage.Create)
         {
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create)
         };

    private class SampleType1 : IDisposable
    {
      public void Dispose() { }
    }

    private class SampleType2 : IDisposable
    {
      public void Dispose() { }
    }
  }
}