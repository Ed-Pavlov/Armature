using System.Diagnostics;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests;

[SetUpFixture]
public class TestFixture
{
  [OneTimeSetUp]
  public void BeforeAllTestsRun()
  {
    AssertionOptions.AssertEquivalencyUsing(
      _ => _.ComparingByValue<IBuildAction>() // don't know why FluentAssertions doesn't compare using polymorphic Equals implementation, but so it goes
            .ComparingByValue<IUnitPattern>() // don't know why FluentAssertions doesn't compare using polymorphic Equals implementation, but so it goes
            .ComparingByMembers<IBuildStackPattern>() // compare pattern tree nodes with build actions which are not included into Equals implementation
             );

    Trace.Listeners.Clear();
    Trace.Listeners.Add(new ConsoleTraceListener());
    Log.Enable(LogLevel.Trace);
  }
}