using JetBrains.dotMemoryUnit;

[assembly: DotMemoryUnit(FailIfRunWithoutSupport = false, SavingStrategy = SavingStrategy.Never)]

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
  internal class IsExternalInit { }
}