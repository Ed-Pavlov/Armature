using System;
using System.Collections.Generic;
using System.Linq;

namespace Armature.Core.Logging
{
  public static class LogExtension
  {
    private const string BuildsequenceIsEmpty = "BuildSequence{{empty}}";

    public static void LogBuildSequence(this IList<UnitInfo> buildSequence, LogLevel logLevel = LogLevel.Verbose)
    {
      if (buildSequence.Count == 0)
        Log.WriteLine(logLevel, BuildsequenceIsEmpty);
      else if (buildSequence.Count == 1)
        Log.WriteLine(logLevel, "BuildSequence{{{0}}}", buildSequence.Last());
      else
        using (Log.Block(logLevel, "BuildSequence"))
          foreach (var unitInfo in buildSequence)
            Log.WriteLine(logLevel, unitInfo.ToString());
    }

    public static void ToLog(this MatchedBuildActions actions, LogLevel logLevel = LogLevel.Verbose)
    {
      if (actions == null)
      {
        Log.WriteLine(logLevel, "not matched");
        return;
      }
      
      foreach (var pair in actions)
        LogStageBuildActions(pair, logLevel);
    }

    private static void LogStageBuildActions(KeyValuePair<object, List<Weighted<IBuildAction>>> stagedActions, LogLevel logLevel)
    {
      var stage = stagedActions.Key;
      var actionsList = stagedActions.Value;

      if (actionsList.Count == 1)
        Log.WriteLine(logLevel, "{0}: {1}", stage.AsLogString(), actionsList[0]);
      else
        using (Log.Block(logLevel, string.Format("[{0}]", stage)))
        {
          actionsList.Sort((l, r) => r.Weight.CompareTo(l.Weight)); // sort by weight descending
          foreach (var action in actionsList)
            Log.WriteLine(logLevel, "{0}", action);
        }
    }
    
    public static string GetShortName(this Type type) => 
      type.IsGenericType 
        ? string.Format("{0}<{1}>", type.GetGenericTypeDefinition().Name, string.Join(", ", type.GenericTypeArguments.Select(_ => _.Name).ToArray()))
        : type.Name;
    
    public static string AsLogString(this object obj) => Log.ToLogString(obj);
  }
}