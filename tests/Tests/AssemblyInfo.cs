using System.Reflection;
using JetBrains.dotMemoryUnit;

#if !DEBUG
[assembly:AssemblyKeyFile(@"..\..\private\armature.snk")]
#endif

[assembly: DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]