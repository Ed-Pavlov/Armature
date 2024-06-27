using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60)]
public class GatherBuildActionOnRealTreeBenchmark
{
  private readonly IBuildStackPattern _treeRoot;

  private readonly BuildSession.Stack _stack = new BuildSession.Stack(new[] {Unit.Of("unobtanium")});

  public GatherBuildActionOnRealTreeBenchmark()
  {
    var builder = new Builder("GatherBuildActionOnBigTreeBenchmark", BuildStage.Cache, BuildStage.Initialize, BuildStage.Create);

    // Treat<I>().AsCreated<C>().AsSingleton();
    const int registrationsCount    = 100;
    const int argsRegistrationCount = 10;

    for(var i = 1; i < registrationsCount; i++)
    {
      var i1      = i;
      var created = (i1 * 10_000).ToString();

      builder.AddNode(CreateNode()).UseBuildAction(new Redirect(Unit.Of(created)), BuildStage.Create);

      builder.AddNode(new IfFirstUnit(new UnitPattern(created)))
              .AddNode(CreateNode())
              .UseBuildAction(new CreateWithFactoryMethod<string>(_ => created.ToString()), BuildStage.Create)
              .UseBuildAction(new Singleton(), BuildStage.Cache);

      continue;

      IfFirstUnit CreateNode() => new IfFirstUnit(new UnitPattern(i1));
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


// Before optimization

// * Summary *

// BenchmarkDotNet=v0.13.3, OS=Windows 10 (10.0.19045.2728)
// 12th Gen Intel Core i7-12700H, 1 CPU, 20 logical and 14 physical cores
//   [Host]               : .NET Framework 4.8 (4.8.4614.0), X64 RyuJIT VectorSize=256
//   .NET 6.0             : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2
//   .NET Framework 4.7.2 : .NET Framework 4.8 (4.8.4614.0), X64 RyuJIT VectorSize=256
//
//
// |             Method |                  Job |              Runtime |     Mean |     Error |    StdDev | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|----------:|----------:|----------:|
// | GatherBuildActions |             .NET 6.0 |             .NET 6.0 | 4.236 us | 0.0832 us | 0.0890 us |         - |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 6.876 us | 0.0474 us | 0.0444 us |         - |
//
// // * Legends *
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 us      : 1 Microsecond (0.000001 sec)

// After introducing _staticMap in BuildStackPatternTree
// * Summary *

// BenchmarkDotNet=v0.13.3, OS=Windows 10 (10.0.19045.2728)
// 12th Gen Intel Core i7-12700H, 1 CPU, 20 logical and 14 physical cores
// .NET SDK=6.0.406
//   [Host]               : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2
//   .NET 6.0             : .NET 6.0.14 (6.0.1423.7309), X64 RyuJIT AVX2
//   .NET Framework 4.7.2 : .NET Framework 4.8 (4.8.4614.0), X64 RyuJIT VectorSize=256
//
//
// |             Method |                  Job |              Runtime |     Mean |    Error |   StdDev | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|---------:|---------:|----------:|
// | GatherBuildActions |             .NET 6.0 |             .NET 6.0 | 10.76 ns | 0.062 ns | 0.058 ns |         - |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 41.42 ns | 0.237 ns | 0.221 ns |         - |
//
// // * Legends *
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 ns      : 1 Nanosecond (0.000000001 sec)
