using Armature;
using Armature.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net70)]
public class GatherBuildActionOnBigTreeBenchmark
{
  private readonly IBuildStackPattern _treeRoot;

  private readonly BuildSession.Stack _stack = new BuildSession.Stack(new[] {Unit.Of("unobtanium")});

  public GatherBuildActionOnBigTreeBenchmark()
  {
    var builder = new Builder(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create);

    // Treat<I>().AsCreated<C>().AsSingleton();
    const int registrationsCount    = 3_000;
    const int argsRegistrationCount = 100;

    for(var i = 1; i < registrationsCount; i++)
    {
      var i1      = i;
      var created = (i1 * 10_000).ToString();

      IfFirstUnit CreateNode() => new IfFirstUnit(new UnitPattern(i1));

      builder.AddNode(CreateNode()).UseBuildAction(new Redirect(Unit.Of(created)), BuildStage.Create);

      builder.AddNode(new IfFirstUnit(new UnitPattern(created)))
              .AddNode(CreateNode())
              .UseBuildAction(new CreateWithFactoryMethod<string>(_ => created.ToString()), BuildStage.Create)
              .UseBuildAction(new Singleton(), BuildStage.Cache);
    }

    // Treat<I>().AsCreated<C>().UsingArguments(1, 2, 3)
    for(int k = 1; k < 4; k++)
    {
      var node = builder.AddNode(new IfFirstUnit(new UnitPattern("arg" + k)));

      for(int i = 1; i < argsRegistrationCount; i++)
      {
        var created = (i * 10_000).ToString();

        node
         .AddNode(new IfFirstUnit(new UnitPattern(created)))
         .AddNode(new IfFirstUnit(new UnitPattern(i)))
         .UseBuildAction(new Instance<string>("arg" + k), BuildStage.Cache);
      }
    }

    _treeRoot = builder;
  }

  [Benchmark]
  public bool GatherBuildActions() => _treeRoot.GatherBuildActions(_stack, out _);
}
