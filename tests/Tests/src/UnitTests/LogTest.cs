using Armature.Core;
using NUnit.Framework;

namespace Tests.UnitTests;

public class LogTest
{
  // [Test]
  // there are no asserts, it's to look at output
  public void indent_in_conditional_mode()
  {
    using var _ = Log.Enable(LogLevel.Trace);
    using var __ = Log.ConditionalMode(LogLevel.Verbose, () => false);

    using(Log.NamedBlock(LogLevel.Verbose, "VerboseBlock"))
    {
      Log.WriteLine(LogLevel.Info, "InfoLine");
      Log.WriteLine(LogLevel.Verbose, "VerboseLine");

      using(Log.NamedBlock(LogLevel.Info, "InfoBlock"))
      {
        Log.WriteLine(LogLevel.Info, "InfoLine");
        Log.WriteLine(LogLevel.Verbose, "VerboseLine");
      }

      Log.WriteLine(LogLevel.Info, "InfoLine");
      Log.WriteLine(LogLevel.Verbose, "VerboseLine");
    }
  }

  [Test]
  public void overlapped_conditional_mode()
  {
    using(Log.Enable(LogLevel.Verbose))
    {
      var rootScope = Log.ConditionalMode(LogLevel.Verbose, () => true);
      var innerScope = Log.ConditionalMode(LogLevel.Verbose, () => true);

      rootScope.Dispose();
      // innerScope.Dispose();
    }
  }
}