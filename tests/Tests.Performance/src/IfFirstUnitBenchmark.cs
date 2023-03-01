using Armature.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472, LaunchCount, WarmupCount, IterationCount, InvocationCount)]
[SimpleJob(RuntimeMoniker.Net70, LaunchCount, WarmupCount, IterationCount, InvocationCount)]
public class IfFirstUnitBenchmark
{
  private const int LaunchCount    = -1;
  private const int WarmupCount    = -1;
  private const int IterationCount = -1;
  private const int InvocationCount = -1;

  private readonly BuildSession.Stack _stack = new BuildSession.Stack(new[] {Unit.Of("unobtanium")});

  private readonly IfFirstUnit _target = new IfFirstUnit(new UnitPattern("kind"));

  [Benchmark]
  public bool GatherBuildActions() => _target.GatherBuildActions(_stack, out _, 0);
}

// |             Method |                  Job |              Runtime |     Mean |    Error |   StdDev |   Gen0 | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|---------:|---------:|-------:|----------:|
// | GatherBuildActions |             .NET 7.0 |             .NET 7.0 | 44.96 ns | 0.702 ns | 0.548 ns | 0.0229 |     192 B |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 67.70 ns | 1.137 ns | 1.064 ns | 0.0305 |     193 B |

// |             Method |                  Job |              Runtime |     Mean |    Error |   StdDev |   Gen0 | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|---------:|---------:|-------:|----------:|
// | GatherBuildActions |             .NET 7.0 |             .NET 7.0 | 39.37 ns | 0.489 ns | 0.382 ns | 0.0191 |     160 B |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 64.03 ns | 0.680 ns | 0.636 ns | 0.0254 |     160 B |

// |             Method |                  Job |              Runtime |     Mean |    Error |   StdDev |   Gen0 | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|---------:|---------:|-------:|----------:|
// | GatherBuildActions |             .NET 7.0 |             .NET 7.0 | 32.38 ns | 0.271 ns | 0.254 ns | 0.0076 |      64 B |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 55.76 ns | 0.218 ns | 0.182 ns | 0.0102 |      64 B |

// |             Method |                  Job |              Runtime |     Mean |    Error |   StdDev | Allocated |
// |------------------- |--------------------- |--------------------- |---------:|---------:|---------:|----------:|
// | GatherBuildActions |             .NET 7.0 |             .NET 7.0 | 25.76 ns | 0.072 ns | 0.068 ns |         - |
// | GatherBuildActions | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 51.51 ns | 0.708 ns | 0.663 ns |         - |
