using System;
using System.Collections;
using System.Linq;

namespace Armature.Core.Logging
{
  public static class LogExtension
  {
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

    public static void ToLog(this WeightedBuildActionBag? actions, LogLevel logLevel)
      => Log.Execute(
        logLevel,
        () =>
        {
          if(actions is null)
            Log.WriteLine(logLevel, "No matched actions");
          else
            foreach(var pair in actions)
            {
              var stage       = pair.Key;
              var actionsList = pair.Value;

              foreach(var action in actionsList)
                Log.WriteLine(logLevel, "{0}{{ Stage={1}, Weight={2:n0} }}", action.Entity, stage, action.Weight);
            }
        });

    public static void ToLog(this Exception exc, Func<string> getTitle)
      => Log.Execute(
        LogLevel.Info,
        () =>
        {
          using(Log.Block(LogLevel.Info, getTitle))
          {
            Log.WriteLine(LogLevel.Info, exc.ToString);

            Log.WriteLine(LogLevel.Info, ExceptionConst.Data);

            foreach(DictionaryEntry entry in exc.Data)
              Log.WriteLine(LogLevel.Info, "{0}: {1}", entry.Key, entry.Value);
          }
        });
  }
}
