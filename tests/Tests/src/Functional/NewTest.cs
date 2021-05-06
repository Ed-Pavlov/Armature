using Armature;
using Armature.Core;
using Armature.Core.Logging;
using NUnit.Framework;

namespace Tests.Functional
{
  public class NewTest
  {
    [Test]
    public void ExceptionLog()
    {
      var builder = new Builder(BuildStage.Create);

      builder
       .TreatAll()
       .InjectInto(Constructor.WithMaxParametersCount())
       .UsingArguments(AutoBuildByParameter.Type);

      builder.Treat<IA1>().AsCreated<A1>();

      // builder.Treat<IB1>().AsCreatedWith<IC1>(v => new B1(v));
      builder.Treat<IB1>().AsCreated<B1>();
      builder.Treat<IC1>().AsCreated<C1>();
      builder.Treat<ISubject>().AsCreated<Subject>();

      Log.Enabled(LogLevel.Trace);

      builder.Build<ISubject>();
    }

    // @formatter: off
    public interface IA1 { }

    public interface IB1 { }

    public interface IC1 { }

    public class A1 : IA1
    {
      public A1(IB1 b) { }
    }

    public class B1 : IB1
    {
      public B1(IC1 c) { }
    }

    public class C1 : IC1
    {
      public C1() { }
    }

    public interface ISubject { }

    public class Subject : ISubject
    {
      public Subject(IA1 a) { }
    }

    // @formatter: on
  }
}
