using System.Diagnostics;
using Armature.Core.Logging;
using NUnit.Framework;

namespace Tests.Extensibility;

[SetUpFixture]
public class TestFixture
{
  [OneTimeSetUp]
  public void BeforeAllTestsRun()
  {
    Trace.Listeners.Clear();
    Trace.Listeners.Add(new ConsoleTraceListener());
    Log.Enabled(LogLevel.Trace);
  }
}