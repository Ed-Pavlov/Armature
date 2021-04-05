using System;
using System.Collections.Generic;
using System.Linq;

namespace Armature.Core.Logging
{
  public static class LogExtension
  {
    private const string BuildSequenceIsEmpty = "BuildSequence{{empty}}";

    public static string GetShortName(this Type type)
      => type.IsGenericType
           ? string.Format(
             "{0}<{1}>",
             type.GetGenericTypeDefinition().Name,
             string.Join(", ", type.GenericTypeArguments.Select(_ => _.Name).ToArray()))
           : type.Name;

    public static void ToLog(this IList<UnitInfo> buildSequence, LogLevel logLevel = LogLevel.Verbose)
      => logLevel.ExecuteIfEnabled(
        () =>
        {
          if(buildSequence.Count == 0)
            Log.WriteLine(logLevel, BuildSequenceIsEmpty);
          else if(buildSequence.Count == 1)
            Log.WriteLine(logLevel, "BuildSequence{{{0}}}", buildSequence.Last());
          else
            using(Log.Block(logLevel, "BuildSequence"))
            {
              foreach(var unitInfo in buildSequence)
                Log.WriteLine(logLevel, unitInfo.ToString());
            }
        });

    public static void ToLog(this MatchedBuildActions? actions, LogLevel logLevel = LogLevel.Verbose)
      => logLevel.ExecuteIfEnabled(
        () =>
        {
          if(actions is null)
          {
            Log.WriteLine(logLevel, "not matched");

            return;
          }

          foreach(var pair in actions)
            LogMatchedBuildActions(pair, logLevel);
        });

    private static void LogMatchedBuildActions(KeyValuePair<object, List<Weighted<IBuildAction>>> stagedActions, LogLevel logLevel)
      => logLevel.ExecuteIfEnabled(
        () =>

        {
          var stage       = stagedActions.Key;
          var actionsList = stagedActions.Value;

          if(actionsList.Count == 1)
            Log.WriteLine(logLevel, "{0}: {1}", stage, actionsList[0]);
          else
            using(Log.Block(logLevel, string.Format("[{0}]", stage)))
            {
              actionsList.Sort((l, r) => r.Weight.CompareTo(l.Weight)); // sort by weight descending

              foreach(var action in actionsList)
                Log.WriteLine(logLevel, "{0}", action);
            }
        });
  }
}
