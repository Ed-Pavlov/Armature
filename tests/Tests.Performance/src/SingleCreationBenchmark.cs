using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net70)]
public class SingleCreationBenchmark
{
  private readonly object[] _args = {"1", "2", "3", "4", "5", "6", "7"};

  [Benchmark]
  public void CreateByConstructorInfo()
  {

    var ctor  = typeof(Subject).GetConstructors().Single(_ => _.GetParameters().Length > 0);
    var value = ctor.Invoke(_args);
  }

  [Benchmark]
  public void CreateByCompiledExpression()
  {
    var ctor    = typeof(Subject).GetConstructors().Single(_ => _.GetParameters().Length > 0);
    var factory = BuildFactoryExpression(ctor);
    var value   = factory(_args);
  }

  private static Func<object[], object> BuildFactoryExpression(ConstructorInfo ctor)
  {
    var parameters = ctor.GetParameters(); // Get the parameters of the constructor
    var args       = new Expression[parameters.Length];
    var param      = Expression.Parameter(typeof(object[])); // The object[] parameter to the Func

    // get the item from the array in the parameter and cast it to the correct type for the constructor
    for(var i = 0; i != parameters.Length; ++i)
      args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), parameters[i].ParameterType);

    return Expression
          .Lambda<Func<object[], object>>(Expression.New(ctor, args), param)
          .Compile();
  }

  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  [SuppressMessage("ReSharper", "UnusedParameter.Local")]
  private class Subject
  {
    public Subject() { }

    public Subject(string s1, string s2, string s3, string s4, string s5, string s6, string s7) { }
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
// |                     Method |                  Job |              Runtime |         Mean |       Error |      StdDev |   Gen0 |   Gen1 | Allocated |
// |--------------------------- |--------------------- |--------------------- |-------------:|------------:|------------:|-------:|-------:|----------:|
// |    CreateByConstructorInfo |             .NET 7.0 |             .NET 7.0 |     175.3 ns |     2.06 ns |     1.93 ns | 0.0305 |      - |     256 B |
// | CreateByCompiledExpression |             .NET 7.0 |             .NET 7.0 | 259,792.1 ns | 2,528.32 ns | 2,365.00 ns | 0.4883 |      - |    7614 B |
// |    CreateByConstructorInfo | .NET Framework 4.7.2 | .NET Framework 4.7.2 |     554.8 ns |     3.74 ns |     3.32 ns | 0.0401 |      - |     257 B |
// | CreateByCompiledExpression | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 383,656.8 ns | 1,628.96 ns | 1,523.73 ns | 1.9531 | 0.9766 |   13228 B |
