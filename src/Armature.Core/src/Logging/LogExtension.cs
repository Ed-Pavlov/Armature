using System;
using System.Collections;
using System.IO;
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
          if(exc.Data.Contains(ExceptionConst.Logged)) return;
          exc.Data.Add(ExceptionConst.Logged, true);
            
          Log.WriteLine(LogLevel.Info, "");

          using(Log.Block(LogLevel.Info, getTitle))
          {
            var number    = 1;
            var exception = exc;

            while(exception is not null)
            {
              using(Log.Block(LogLevel.Info, $"Exception #{number}"))
              {
                Log.WriteLine(LogLevel.Info, "Exception.Message");
                WriteText(LogLevel.Info, exception.Message);

                Log.WriteLine(LogLevel.Info, "");
                Log.WriteLine(LogLevel.Info, "Exception.StackTrace");
                WriteText(LogLevel.Info, exception.StackTrace);


                if(exception.Data.Count > 0)
                {
                  Log.WriteLine(LogLevel.Info, "");
                  Log.WriteLine(LogLevel.Info, "Exception.Data");

                  foreach(DictionaryEntry entry in exception.Data)
                    Log.WriteLine(LogLevel.Info, "{0}: {1}", entry.Key, entry.Value);
                }

                exception = exception.InnerException;
                number++;
              }
            }
          }
        });

    private static void WriteText(LogLevel logLevel, string text)
    {
      using var stringReader = new StringReader(text);

      var line = stringReader.ReadLine();

      while(line is not null)
      {
        Log.WriteLine(logLevel, line);
        line = stringReader.ReadLine();
      }
    }
  }
}
