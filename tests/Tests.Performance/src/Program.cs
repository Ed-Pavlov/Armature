using BenchmarkDotNet.Running;
using Tests.Performance;

// var summary = BenchmarkRunner.Run<IfFirstUnitBenchmark>();
// var summary = BenchmarkRunner.Run<MassCreationBenchmark>();
// var summary = BenchmarkRunner.Run<SingleCreationBenchmark>();
var summary = BenchmarkRunner.Run<AddOrGetNodeBenchmark>();