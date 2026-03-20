using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.Functional.WeightTests;

public class AmendWeightTest
{
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

  [Test]
  public void should_build_open_generic_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.TreatOpenGeneric(typeof(IGenericInterface<>)).As(typeof(GenericImpl<>));

    target
     .TreatOpenGeneric(typeof(IGenericInterface<>))
     .AmendWeight(10)
     .As(typeof(GenericTestImpl<>));

    // --act
    var actual = target.Build<IGenericInterface<int>>();

    // --assert
    actual.Should().BeOfType<GenericTestImpl<int>>();
  }

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

  [Test]
  public void should_build_contextual_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.Treat<IInterface>().AsCreated<RealImpl>();

    target
     .Building<Context>()
     .Treat<IInterface>()
     .AmendWeight(10)
     .AsCreated<TestImpl>();

    // --act
    var actual = target.Build<Context>();

    // --assert
    actual.Should().NotBeNull();
    actual.Dependency.Should().BeOfType<TestImpl>();
  }

  [Test]
  public void should_inject_property_with_more_weight()
  {
    // --arrange
    var target = CreateTarget();

    target.Treat<IInterface>().AsInstance(new RealImpl());

    target
     .Treat<InjectProperty>()
     .UsingArguments(
        ForProperty
         .Named(nameof(InjectProperty.Dependency))
         .UseTag(nameof(TestImpl)));

    target
     .Treat<InjectProperty>()
     .Using()
     .UsingArguments(
        ForProperty
         .Named(nameof(InjectProperty.Dependency))
         .AmendWeight(10)
         .UseTag(nameof(TestImpl)));

    target.Treat<IInterface>(nameof(TestImpl)).As<TestImpl>();

    // --act
    var actual = target.Build<InjectProperty>();

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

  private class Context
  {
    public readonly IInterface Dependency;
    public Context(IInterface dependency) => Dependency = dependency;
  }

  private class InjectProperty
  {
    [Inject]
    public IInterface Dependency { get; set; }
  }

  private class InjectMethod
  {
    public IInterface Dependency                    { get; private set; }
    public void       Inject(IInterface dependency) => Dependency = dependency;
  }

  private static Builder CreateTarget()
    => new("test", BuildStage.Cache, BuildStage.Create)
       {
         new IfFirstUnit(new IsConstructor()).UseBuildAction(new GetConstructorWithMaxParametersCount(), BuildStage.Create),

         new IfFirstUnit(new IsPropertyInfoCollection()).UseBuildAction(new BuildListArgumentForProperty(), BuildStage.Create),
         new IfFirstUnit(new IsPropertyArgument()).UseBuildAction(
           new TryInOrder(
             new BuildArgumentByPropertyInjectPoint(),
             new BuildArgumentByPropertyType()
           ),
           BuildStage.Create),

         new IfFirstUnit(new IsParameterInfoArray()).UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
         new IfFirstUnit(new IsParameterArgument()).UseBuildAction(
           new TryInOrder(
             new BuildArgumentByParameterInjectPoint(),
             new BuildArgumentByParameterType(),
             new GetParameterDefaultValue()),
           BuildStage.Create),
       };
}
