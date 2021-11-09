using System.Reflection;

namespace Armature.Core.Sdk;

public static class LogConst
{
  public const string Matched = "Matched: {0}";

  public static string BuildAction_Process(IBuildAction buildAction) => $"{buildAction.GetType().GetShortName().QuoteIfNeeded()}.{nameof(IBuildAction.Process)}";
  public static string BuildAction_PostProcess(IBuildAction buildAction) => $"{buildAction.GetType().GetShortName().QuoteIfNeeded()}.{nameof(IBuildAction.PostProcess)}";

  public static void Log_Constructors(ConstructorInfo[] constructors)
    => Log.Execute(
      LogLevel.Trace,
      () =>
      {
        if(constructors.Length < 2)
          Log.Execute(
            LogLevel.Trace,
            () => Log.WriteLine(LogLevel.Trace, $"Constructor: {(constructors.Length == 0 ? "null" : $"{constructors[0]}".Quote())}"));
        else
          using(Log.IndentBlock(LogLevel.Trace, "Constructors: ", "[]"))
            foreach(var constructor in constructors)
              Log.WriteLine(LogLevel.Trace, $"{constructor}".Quote());
      });
}