using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BeatyBit.Armature.Core.Sdk;
using JetBrains.Lifetimes;
using NUnit.Framework;

namespace Tests.Extensibility.LifetimeRegistration;

public class Test
{
  [Test]
  public void test()
  {
    var builder  = CreateTarget();
    var lifetime = new Lifetime();

    builder
       .Treat<Interface>()
       .AsCreatedWith<Lifetime>(lt => new Impl(lt));

    builder
       .Treat<Subject>()
       .AsIs()
       .AsSingleton(lifetime);



    builder.PrintToLog();


    // var actual = builder.Build<Subject>();
  }

  private static Builder CreateTarget()
    => new Builder("Root Builder", BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
       {
           // choose which constructor to call to instantiate an object
           new IfFirstUnit(new IsConstructor())
              .UseBuildAction(
                   new TryInOrder
                   {
                       new GetConstructorByInjectPoint(),         // constructor marked with [Inject] attribute has more priority
                       new GetConstructorWithMaxParametersCount() // constructor with the largest number of parameters has less priority
                   },
                   BuildStage.Create),

           new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create), // build arguments for a constructor/method in the order of their parameters

           // build each argument for a constructor/method
           new IfFirstUnit(new IsParameterArgument())
              .UseBuildAction(
                   new TryInOrder
                   {
                     new BuildArgumentByParameterInjectPoint(), // parameter marked with [Inject] attribute has more priority
                     new BuildArgumentByParameterType(),        // if not, try to build it by type
                     new GetParameterDefaultValue()
                   },
                   BuildStage.Create),

           new IfFirstUnit(Unit.Any)
            .UseBuildAction(new InjectDependenciesIntoProperties(), BuildStage.Initialize), // try to inject dependencies into property of any built unit

           new IfFirstUnit(new IsPropertyInfoCollection())
            .UseBuildAction(new GetPropertyListByInjectAttribute(), BuildStage.Create), // inject dependencies into all properties marked with [Inject] attribute

           new IfFirstUnit(new IsPropertyArgument())
            .UseBuildAction(new BuildArgumentByPropertyInjectPoint(), BuildStage.Create) // use a property type and InjectAttribute.Tag as UnitId.Tag to build argument for a property
       };

  public interface Interface { }

  public class Impl : Interface
  {
    public Lifetime Lifetime { get; }

    public Impl(Lifetime lifetime) => Lifetime = lifetime;
  }

  public class Subject
  {
    public Subject(Interface i) { }
  }
}