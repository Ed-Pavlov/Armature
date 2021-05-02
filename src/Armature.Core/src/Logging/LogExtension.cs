using System;
using System.Collections.Generic;
using System.Linq;

namespace Armature.Core.Logging
{
  public static class LogExtension
  {
    private const string BuildSequenceIsEmpty = "BuildSequence{ empty }";

    public static string GetFullName(this Type type)
      => type.IsGenericType
           ? string.Format(
             "{0}<{1}>",
             type.GetGenericTypeDefinition().FullName,
             string.Join(", ", type.GenericTypeArguments.Select(GetFullName).ToArray()))
           : type.FullName!;

    public static string GetShortName(this Type type)
      => type.IsGenericType
           ? string.Format(
             "{0}<{1}>",
             type.GetGenericTypeDefinition().Name,
             string.Join(", ", type.GenericTypeArguments.Select(GetShortName).ToArray()))
           : type.Name;

    public static void ToLog(this WeightedBuildActionBag? actions, LogLevel logLevel = LogLevel.Verbose)
      => logLevel.ExecuteIfEnabled(
        () =>
        {
          if(actions is null)
          {
            Log.WriteLine(logLevel, LogConst.NoMatch);
            return;
          }

          foreach(var pair in actions)
            LogBuildActionBag(pair, logLevel);
        });

    private static void LogBuildActionBag(KeyValuePair<object, List<Weighted<IBuildAction>>> stagedActions, LogLevel logLevel)
      => logLevel.ExecuteIfEnabled(
        () =>
        {
          var stage       = stagedActions.Key;
          var actionsList = stagedActions.Value;

          foreach(var action in actionsList)
            Log.WriteLine(logLevel, "{0}{{ Stage={1}, Weight={2:n0} }}", action.Entity, stage, action.Weight);
        });
  }
}
