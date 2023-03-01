using Armature;
using Armature.Core;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net70)]
public class AddOrGetNodeBenchmark
{
  [Benchmark]
  public static IContextTuner AddOrGetNode()
  {
    var builder = new Builder("stage");
    return builder.Treat<string?>().AsCreatedWith(() => null).AsSingleton();
  }


  private static BuildingTuner<object?> Treat(IBuildStackPattern pattern, UnitId unitId)
  {
    if(pattern is null) throw new ArgumentNullException(nameof(pattern));

    var unitPattern = new UnitPatternWrapper(new UnitPattern(unitId.Kind, unitId.Tag));
    return new BuildingTuner<object?>(new RootTuner(pattern), () => new SkipTillUnit(unitPattern), unitPattern);
  }

  private class UnitPatternWrapper : IUnitPattern
  {
    private readonly IUnitPattern _impl;

    public UnitPatternWrapper(IUnitPattern impl) => _impl = impl;

    public static long EqualsCallsCount      { get; private set; }
    public static long GetHashCodeCallsCount { get; private set; }

    public bool Matches(UnitId unitId) => _impl.Matches(unitId);

    private bool Equals(IUnitPattern? other)
    {
      EqualsCallsCount++;

      return _impl.Equals(other);
    }

    public override int GetHashCode()
    {
      // ReSharper disable once NonReadonlyMemberInGetHashCode
      GetHashCodeCallsCount++;

      return _impl.GetHashCode();
    }

    public override bool Equals(object? obj) => Equals(obj as UnitPattern);
  }
}


// BenchmarkDotNet=v0.13.3, OS=Windows 10 (10.0.19045.2486)
// Intel Core i9-9980HK CPU 2.40GHz, 1 CPU, 16 logical and 8 physical cores
// .NET SDK=7.0.100-rc.2.22477.23
//   [Host]               : .NET 7.0.0 (7.0.22.47203), X64 RyuJIT AVX2
//   .NET 7.0             : .NET 7.0.0 (7.0.22.47203), X64 RyuJIT AVX2
//   .NET Framework 4.7.2 : .NET Framework 4.8 (4.8.4515.0), X64 RyuJIT VectorSize=256
//
//
// |       Method |                  Job |              Runtime |     Mean |   Error |  StdDev |   Gen0 |   Gen1 | Allocated |
// |------------- |--------------------- |--------------------- |---------:|--------:|--------:|-------:|-------:|----------:|
// | AddOrGetNode |             .NET 7.0 |             .NET 7.0 | 422.3 ns | 3.37 ns | 2.63 ns | 0.1659 | 0.0010 |   1.36 KB |
// | AddOrGetNode | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 472.4 ns | 0.85 ns | 0.67 ns | 0.2370 | 0.0019 |   1.46 KB |
