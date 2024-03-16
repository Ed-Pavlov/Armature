using BenchmarkDotNet.Running;
using Tests.Performance;

// var summary = BenchmarkRunner.Run<IfFirstUnitBenchmark>();
// var summary = BenchmarkRunner.Run<MassCreationBenchmark>();
// var summary = BenchmarkRunner.Run<SingleCreationBenchmark>();
// var summary = BenchmarkRunner.Run<AddOrGetNodeBenchmark>();
// var summary = BenchmarkRunner.Run<GatherBuildActionOnBigTreeBenchmark>();
var summary = BenchmarkRunner.Run<GatherBuildActionOnRealTreeBenchmark>();

//     Trace.Listeners.Clear();
//     Trace.Listeners.Add(new ConsoleTraceListener());
//     Log.Enable(LogLevel.Trace);
// new GatherBuildActionOnBigTreeBenchmark();