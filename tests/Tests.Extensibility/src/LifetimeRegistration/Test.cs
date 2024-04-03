using Armature;
using Armature.Core;
using Armature.Core.Sdk;
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
    => new("test", BuildStage.Cache, BuildStage.Create)
       {
           // inject into constructor
           new IfFirstUnit(new IsConstructor())
              .UseBuildAction(
                   new TryInOrder
                   {
                       new GetConstructorByInjectPoint(),       // constructor marked with [Inject] attribute has more priority
                       new GetConstructorWithMaxParametersCount() // constructor with largest number of parameters has less priority
                   },
                   BuildStage.Create),
           new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
           new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(
                   new TryInOrder {Static.Of<BuildArgumentByParameterInjectPoint>(), Static.Of<BuildArgumentByParameterType>()},
                   BuildStage.Create)
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