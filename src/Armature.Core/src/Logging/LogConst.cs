using System;
using System.Runtime.CompilerServices;

namespace Armature.Core;

public static class LogConst
{
  public const string Matched = "Matched: {0}";

  public const string LoggingSubsystemError = "LoggingSubsystem_Error";

  public static string ArmatureExceptionPostfix(string options = "") =>
    Environment.NewLine
  + $"See {nameof(Exception)}.{nameof(Exception.Data)}{options} for details or enable logging using {nameof(Log)}.{nameof(Log.Enable)} to investigate the error.";
}