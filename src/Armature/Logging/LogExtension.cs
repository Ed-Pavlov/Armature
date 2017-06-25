using System.Collections.Generic;
using Armature.Common;
using Armature.Core;

namespace Armature.Logging
{
  public static class LogExtension
  {
    public static void LogBuildSequence(this IList<UnitInfo> buildSequence)
    {
      if (buildSequence.Count == 0)
        Log.Info("BuildSequence=null");
      else
        LogBuildSequence(buildSequence.GetTail(0));
    }

    public static void LogBuildSequence(this ArrayTail<UnitInfo> buildSequence, LogLevel logLevel = LogLevel.Info)
    {
      if (buildSequence.Length == 0)
        Log.WriteLine(logLevel, "BuildSequence=null");
      else if (buildSequence.Length == 1)
        Log.WriteLine(logLevel, "BuildSequence={0}", buildSequence.GetLastItem());
      else
      {
        using (Log.Block("BuildSequence", logLevel))
          for (var i = 0; i < buildSequence.Length; i++)
            Log.Info(buildSequence[i].ToString());
      }
    }

    public static void LogDoesNotMatch(this IBuildStep match, ArrayTail<UnitInfo> buildSequence, string format = null, params object[] param)
    {
      if (format == null)
      {
        Log.Info("{0}: not matched", match.GetType().Name);
        buildSequence.LogBuildSequence(LogLevel.Verbose);
      }
      else
      {
        using(Log.Block(string.Format("{0}: not matched", match.GetType().Name)))
        {
          Log.Info("Parameter");
        }
      }
    }

    public static void LogInfo(this IBuildStep match, string format, params object[] param)
    {
      Log.Info("{0}: {1}", match.GetType().Name, string.Format(format, param));
    }

    public static void LogBuildStepMatch(this IBuildStep buildStep, ArrayTail<UnitInfo> buildSequence)
    {
      using (Log.Block(buildStep.GetType().Name))
      {
        Log.Info("matches!");
//        Log.Info("Weight={0}", buildAction.Weight);
        buildSequence.LogBuildSequence();
      }
    }
  }
}