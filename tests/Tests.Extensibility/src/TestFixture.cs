using System.Diagnostics;
using Armature.Core;
using NUnit.Framework;

namespace Tests.Extensibility;

[SetUpFixture]
public class TestFixture
{
  [OneTimeSetUp]
  public void BeforeAllTestsRun()
  {
#if NETCOREAPP3_1_OR_GREATER
    Trace.Listeners.Clear();
    Trace.Listeners.Add(new ConsoleTraceListener());
    Log.Enable(LogLevel.Trace);
#endif
  }
}