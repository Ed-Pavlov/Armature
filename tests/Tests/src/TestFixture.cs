using Armature.Core.Logging;
using NUnit.Framework;

namespace Tests;

[SetUpFixture]
public class TestFixture
{
  [OneTimeSetUp]
  public void BeforeAllTestsRun() => Log.Enabled(LogLevel.Trace);
}