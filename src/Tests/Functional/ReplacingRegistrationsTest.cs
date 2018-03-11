using System;
using System.IO;
using Armature;
using Armature.OverrideSugar;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class ReplacingRegistrationsTest
  {
    [Test]
    public void ReplaceSingleton()
    {
      var builder = FunctionalTestHelper.CreateBuilder();

      builder
        .Treat<IDisposable>()
        .As<MemoryStream>()
        .UsingParameterlessConstructor()
        .AsSingleton();

      builder
        .Override<IDisposable>()
        .As<MemoryStream>()
        .UsingConstructorWithParameters(typeof(int))
        .UsingParameters(10)
        .AsSingleton();

      builder.Build<IDisposable>().Should().BeOfType<MemoryStream>().Which.Capacity.Should().Be(10);
    }
  }
}