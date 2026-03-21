using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;
// ReSharper disable UnusedTypeParameter

namespace Armature.Test.Functional.WeightTests;

/// <summary>
/// Tests for AmendWeight functionality across all tuner types:
/// </summary>
public class AmendWeightTest
{
  // Test for IBuildingTuner<T>.AmendWeight
  [Test]
  public void should_build_unit_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.Treat<IInterface>().AsCreated<RealImpl>();

    target
     .Treat<IInterface>()
     .AmendWeight(10)
     .AsCreated<TestImpl>();

    // --act
    var actual = target.Build<IInterface>();

    // --assert
    actual.Should().BeOfType<TestImpl>();
  }

  // Test for BuildingOpenGenericTuner (inherits from BuildingTuner<T> which has AmendWeight through IBuildingTuner<T>)
  [Test]
  public void should_build_open_generic_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.TreatOpenGeneric(typeof(IGenericInterface<>)).AsCreated(typeof(GenericImpl<>));

    target
     .TreatOpenGeneric(typeof(IGenericInterface<>))
     .AmendWeight(10)
     .AsCreated(typeof(GenericTestImpl<>));

    // --act
    var actual = target.Build<IGenericInterface<int>>();

    // --assert
    actual.Should().BeOfType<GenericTestImpl<int>>();
  }

  // Test for ISubjectTuner.AmendWeight via TreatInheritorsOf
  [Test]
  public void should_build_inheritor_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.TreatInheritorsOf<IInterface>().AsCreated<RealImpl>();

    target
     .TreatInheritorsOf<IInterface>()
     .AmendWeight(10)
     .AsCreated<TestImpl>();

    // --act
    var actual = target.Build<IInterface>();

    // --assert
    actual.Should().BeOfType<TestImpl>();
  }

  // Test for PropertyArgumentTuner.AmendWeight
  [Test]
  public void should_inject_property_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.Treat<IInterface>(nameof(RealImpl)).AsCreated<RealImpl>();
    target.Treat<IInterface>(nameof(TestImpl)).AsCreated<TestImpl>();
    target.Treat<InjectProperty>().AsIs();

    target
     .Treat<InjectProperty>()
     .UsingInjectionPoints(Property.Named(nameof(InjectProperty.Dependency)))
     .UsingArguments(
        ForProperty
         .Named(nameof(InjectProperty.Dependency))
         .UseTag(nameof(RealImpl)));

    target
     .Treat<InjectProperty>()
     .UsingInjectionPoints(Property.Named(nameof(InjectProperty.Dependency)))
     .UsingArguments(
        ForProperty
         .Named(nameof(InjectProperty.Dependency))
         .AmendWeight(10)
         .UseTag(nameof(TestImpl)));

    // --act
    var actual = target.Build<InjectProperty>();

    // --assert
    actual.Should().NotBeNull();
    actual.Dependency.Should().BeOfType<TestImpl>();
  }

  // Test for MethodArgumentTuner.AmendWeight (using constructor parameters)
  [Test]
  public void should_inject_constructor_parameter_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.Treat<IInterface>(nameof(RealImpl)).AsCreated<RealImpl>();
    target.Treat<IInterface>(nameof(TestImpl)).AsCreated<TestImpl>();
    target.Treat<Context>().AsIs();

    target
     .Treat<Context>()
     .UsingArguments(
        ForParameter
         .OfType<IInterface>()
         .UseTag(nameof(RealImpl)));

    target
     .Treat<Context>()
     .UsingArguments(
        ForParameter
         .OfType<IInterface>()
         .AmendWeight(10)
         .UseTag(nameof(TestImpl)));

    // --act
    var actual = target.Build<Context>();

    // --assert
    actual.Should().NotBeNull();
    actual.Dependency.Should().BeOfType<TestImpl>();
  }

  // Test for IAllTuner.AmendWeight
  [Test]
  public void should_use_all_tuner_amend_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.Treat<Context>().AsIs();
    target
     .Building<Context>()
     .TreatAll()
     .UsingArguments(new RealImpl());

    target
     .Building<Context>()
     .TreatAll()
     .AmendWeight(10)
     .UsingArguments(new TestImpl());

    // --act
    var actual = target.Build<Context>();

    // --assert
    actual.Should().NotBeNull();
    actual.Dependency.Should().BeOfType<TestImpl>();
  }

  private interface IInterface { }

  private class RealImpl : IInterface { }

  private class TestImpl : IInterface { }

  private interface IGenericInterface<T> { }

  private class GenericImpl<T> : IGenericInterface<T> { }

  private class GenericTestImpl<T> : IGenericInterface<T> { }

  [UsedImplicitly]
  private class Context
  {
    public readonly IInterface Dependency;
    public Context(IInterface dependency) => Dependency = dependency;
  }

  [UsedImplicitly]
  private class InjectProperty
  {
    [Inject]
    public IInterface Dependency { get; set; } = null!;
  }

  private static Builder CreateTarget()
    => new("test", BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
       {
         new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),

         new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
         new IfFirstUnit(new IsParameterArgument()).UseBuildAction(
           new TryInOrder(
             new BuildArgumentByParameterInjectPoint(),
             new BuildArgumentByParameterType(),
             new GetParameterDefaultValue()),
           BuildStage.Create),
       };
}
