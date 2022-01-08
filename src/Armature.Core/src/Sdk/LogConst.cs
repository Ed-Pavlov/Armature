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
}