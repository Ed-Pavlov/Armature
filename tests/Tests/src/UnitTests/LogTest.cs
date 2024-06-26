using System;
using BeatyBit.Armature.Core;
using NUnit.Framework;

namespace Tests.UnitTests;

public class LogTest
{
  // [Test]
  // there are no asserts, it's to look at output
  public void indent_in_conditional_mode()
  {
    using var _ = Log.Enable(LogLevel.Trace);
    // using var log = Log.ConditionalMode(LogLevel.Verbose);
    // log.IsConfirmed = false;

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

  // [Test]
  // public void overlapped_conditional_mode()
  // {
  //   using(Log.Enable(LogLevel.Verbose))
  //   {
  //     var rootScope = Log.ConditionalMode(LogLevel.Verbose);
  //     var innerScope = Log.ConditionalMode(LogLevel.Verbose);
  //
  //     rootScope.IsConfirmed  = true;
  //     innerScope.IsConfirmed = true;
  //
  //     rootScope.Dispose();
  //     // innerScope.Dispose();
  //   }
  // }

  [Test]
  public void timestamp()
  {
    var t = DateTime.Now;

    Console.WriteLine(t.ToString("yyyy-mm-dd HH:mm:ss.fff"));
    Console.WriteLine(Environment.CurrentManagedThreadId.ToString());
  }
}