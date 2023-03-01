using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Armature.Core.Sdk;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Tests.Performance;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net472)]
[SimpleJob(RuntimeMoniker.Net70)]
public class MassCreationBenchmark
{
  private readonly object[]               _emptyArgs            = Empty<object>.Array;
  private readonly object[]               _args                 = {"1", "2", "3", "4", "5", "6", "7"};
  private readonly ConstructorInfo        _constructorInfo      = typeof(Subject).GetConstructors().Single(_ => _.GetParameters().Length > 0);
  private readonly ConstructorInfo        _emptyConstructorInfo = typeof(Subject).GetConstructors().Single(_ => _.GetParameters().Length == 0);
  private readonly Func<object[], object> _ctorFactory;
  private readonly Func<object[], object> _emptyCtorFactory;

  public MassCreationBenchmark()
  {
    _emptyCtorFactory = BuildFactoryExpression(_emptyConstructorInfo);
    _ctorFactory      = BuildFactoryExpression(_constructorInfo);
  }

  [Benchmark]
  public void CreateByEmptyActivator() => Activator.CreateInstance(typeof(Subject), _emptyArgs);

  [Benchmark]
  public void CreateByActivator() => Activator.CreateInstance(typeof(Subject), _args);

  [Benchmark]
  public void CreateByEmptyConstructorInfo() => _emptyConstructorInfo.Invoke(_emptyArgs);

  [Benchmark]
  public void CreateByConstructorInfo() => _constructorInfo.Invoke(_args);

  [Benchmark]
  public void CreateByEmptyCompiledExpression() => _emptyCtorFactory(_emptyArgs);

  [Benchmark]
  public void CreateByCompiledExpression() => _ctorFactory(_args);

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
// |                          Method |                  Job |              Runtime |       Mean |     Error |    StdDev |   Gen0 | Allocated |
// |-------------------------------- |--------------------- |--------------------- |-----------:|----------:|----------:|-------:|----------:|
// |          CreateByEmptyActivator |             .NET 7.0 |             .NET 7.0 | 177.483 ns | 1.2107 ns | 1.1325 ns | 0.0334 |     280 B |
// |               CreateByActivator |             .NET 7.0 |             .NET 7.0 | 388.055 ns | 5.4554 ns | 6.8994 ns | 0.0629 |     528 B |
// |    CreateByEmptyConstructorInfo |             .NET 7.0 |             .NET 7.0 |  12.740 ns | 0.1878 ns | 0.1757 ns | 0.0029 |      24 B |
// |         CreateByConstructorInfo |             .NET 7.0 |             .NET 7.0 |  89.363 ns | 0.5914 ns | 0.5242 ns | 0.0124 |     104 B |
// | CreateByEmptyCompiledExpression |             .NET 7.0 |             .NET 7.0 |   3.791 ns | 0.0531 ns | 0.0471 ns | 0.0029 |      24 B |
// |      CreateByCompiledExpression |             .NET 7.0 |             .NET 7.0 |   6.681 ns | 0.0486 ns | 0.0431 ns | 0.0029 |      24 B |
// |          CreateByEmptyActivator | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 405.986 ns | 4.9488 ns | 4.6291 ns | 0.0548 |     345 B |
// |               CreateByActivator | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 968.472 ns | 1.8009 ns | 1.5038 ns | 0.0896 |     570 B |
// |    CreateByEmptyConstructorInfo | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 108.259 ns | 1.1614 ns | 1.0863 ns | 0.0038 |      24 B |
// |         CreateByConstructorInfo | .NET Framework 4.7.2 | .NET Framework 4.7.2 | 381.087 ns | 2.3702 ns | 1.9793 ns | 0.0162 |     104 B |
// | CreateByEmptyCompiledExpression | .NET Framework 4.7.2 | .NET Framework 4.7.2 |   7.524 ns | 0.0837 ns | 0.0783 ns | 0.0038 |      24 B |
// |      CreateByCompiledExpression | .NET Framework 4.7.2 | .NET Framework 4.7.2 |  14.339 ns | 0.0531 ns | 0.0497 ns | 0.0038 |      24 B |
