using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net60)]
public class GatherBuildActionOnBigTreeBenchmark
{
  private readonly IBuildStackPattern _treeRoot;

  private readonly BuildSession.Stack _stack = new BuildSession.Stack(new[] {Unit.Of("unobtanium")});

  public GatherBuildActionOnBigTreeBenchmark()
  {
    var builder = new Builder("GatherBuildActionOnBigTreeBenchmark", BuildStage.Cache, BuildStage.Initialize, BuildStage.Create);

    // Treat<I>().AsCreated<C>().AsSingleton();
    const int registrationsCount    = 3_000;
    const int argsRegistrationCount = 100;

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
// |             Method |                  Job |              Runtime |     Mean |   Error |  StdDev | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|--------:|--------:|----------:|
// | GatherBuildActions |             .NET 6.0 |             .NET 6.0 | 125.6 us | 1.51 us | 1.41 us |         - |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 205.0 us | 1.57 us | 1.31 us |         - |
//
// // * Hints *
// Outliers
//   GatherBuildActionOnBigTreeBenchmark.GatherBuildActions: .NET Framework 4.7.2 -> 2 outliers were removed (216.90 us, 218.58 us)

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
// | GatherBuildActions |             .NET 6.0 |             .NET 6.0 | 11.13 ns | 0.056 ns | 0.053 ns |         - |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 41.33 ns | 0.180 ns | 0.159 ns |         - |
//
// // * Hints *
// Outliers
//   GatherBuildActionOnBigTreeBenchmark.GatherBuildActions: .NET 6.0             -> 1 outlier  was  detected (12.13 ns)
//   GatherBuildActionOnBigTreeBenchmark.GatherBuildActions: .NET Framework 4.7.2 -> 1 outlier  was  removed (42.98 ns)
//
// // * Legends *
//   Mean      : Arithmetic mean of all measurements
//   Error     : Half of 99.9% confidence interval
//   StdDev    : Standard deviation of all measurements
//   Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
//   1 ns      : 1 Nanosecond (0.000000001 sec)