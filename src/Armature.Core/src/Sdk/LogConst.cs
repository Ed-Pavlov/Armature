using System;
using System.Runtime.CompilerServices;

namespace Armature.Core.Sdk;

public static class LogConst
{
  public const string Matched = "Matched: {0}";

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string BuildAction_Name(IBuildAction buildAction)
    => $"{buildAction.GetType().GetShortName().QuoteIfNeeded()}";

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string BuildAction_Process(IBuildAction buildAction)
    => $"{BuildAction_Name(buildAction)}.{nameof(IBuildAction.Process)}";

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string BuildAction_PostProcess(IBuildAction buildAction)
    => $"{BuildAction_Name(buildAction)}.{nameof(IBuildAction.PostProcess)}";

  public static string ArmatureExceptionPostfix(string options = "") =>
    Environment.NewLine
  + $"See {nameof(Exception)}.{nameof(Exception.Data)}{options} for details or enable logging using {nameof(Log)}.{nameof(Log.Enable)} to investigate the error.";

  public const string LoggingSubsystemError = "LoggingSubsystem_Error";
}