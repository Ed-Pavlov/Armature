using System;
using Armature;
using Armature.Interface;
using Armature.Logging;
using Armature.Properties;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class UsingConstructorTest
  {
    [Test]
    public void UsingConstructorShouldBeAppliedDirectlyToRegisteredType()
    {
      var target = FunctionalTestHelper.CreateBuilder();

      target
        .Treat<Dependency>()
        .AsIs()
        .UsingParameters("value");

      target
        .Treat<Constructed>()
        .AsIs()
        .UsingInjectPointConstructor(Constructed.ConstructorId);

      var actual = target.Build<Constructed>();

      // --assert
      actual.Dependency.Should().NotBeNull();
      actual.Dependency.ConstructedViaAttributed.Should().BeFalse();
    }
  }

  public class Constructed
  {
    public const string ConstructorId = "poiintid";
    public readonly Dependency Dependency;

    [Inject(ConstructorId)]
    public Constructed([NotNull] Dependency dependency)
    {
      if (dependency == null) throw new ArgumentNullException(nameof(dependency));

      Dependency = dependency;
    }

    public Constructed(string value, int digit) // longest constructor
    {
    }
  }

  public class Dependency
  {
    public readonly bool ConstructedViaAttributed;

    [Inject(Constructed.ConstructorId)]
    public Dependency() => ConstructedViaAttributed = true;

    public Dependency(string value) { }
  }
}