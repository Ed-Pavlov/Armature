using System.Collections.Generic;
using System.Reflection;
using Armature.Common;
using Armature.Core;

namespace Armature.Logging
{
  public static class LogExtension
  {
    public static void LogBuildSequence(this IList<UnitInfo> buildSequence)
    {
      if (buildSequence.Count == 0)
        Log.Info("BuildSequence is empty");
      else
        LogBuildSequence(buildSequence.GetTail(0));
    }

    public static void LogBuildSequence(this ArrayTail<UnitInfo> buildSequence, LogLevel logLevel = LogLevel.Info)
    {
      if (buildSequence.Length == 0)
        Log.WriteLine(logLevel, "BuildSequence is empty");
      else if (buildSequence.Length == 1)
        Log.WriteLine(logLevel, "BuildSequence={0}", buildSequence.GetLastItem());
      else
      {
        using (Log.Block("BuildSequence", logLevel))
          for (var i = 0; i < buildSequence.Length; i++)
            Log.Info(buildSequence[i].ToString());
      }
    }
    
    public static void LogConstructor(this ConstructorInfo constructorInfo, object source)
    {
      Log.Trace("{0}: {1}", source, constructorInfo == null ? "constructor is not found" : string.Format("{0} found", constructorInfo));
    }
  }
}