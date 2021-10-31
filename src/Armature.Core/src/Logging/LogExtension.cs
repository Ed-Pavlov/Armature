using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Armature.Core.Logging
{
  public static class LogExtension
  {
    private static readonly HashSet<char> BadCharacters = new(
      new[] { '$', '"', '{', '}', '[', ']', ':', '=', ',', '+', '#', '`', '^', '?', '!', '@', '*', '&', '/', '\\', ' ', '.' });

    public static string GetFullName(this Type type)
      => (type.IsGenericType
           ? string.Format(
             "{0}<{1}>",
             type.GetGenericTypeDefinition().FullName,
             string.Join(", ", type.GenericTypeArguments.Select(GetFullName).ToArray()))
           : type.FullName!);

    public static string GetShortName(this Type type)
      => (type.IsGenericType
           ? string.Format(
             "{0}<{1}>",
             type.GetGenericTypeDefinition().Name,
             string.Join(", ", type.GenericTypeArguments.Select(GetShortName).ToArray()))
           : type.Name);

    /// <summary>
    ///   Returns the name of <paramref name="type" /> respecting <see cref="Log.LogFullTypeName" /> property
    /// </summary>
    public static string ToLogString(this Type type) => Log.LogFullTypeName ? type.GetFullName() : type.GetShortName();

    /// <summary>
    ///   Returns log representation of object, some objects logs in more friendly form then common <see cref="object.ToString" /> returns
    /// </summary>
    public static string ToLogString(this object? value)
      => value switch
         {
           null                        => "null",
           string str                  => str,
           IEnumerable<UnitId> unitIds => unitIds.ToLogString(),
           ILogable logable            => logable.ToLogString(),
           ILogable1 logable           => logable.ToLogString(),
           IBuildAction buildAction    => buildAction.ToLogString(),
           Type type                   => $"typeof({(Log.LogFullTypeName ? type.GetFullName() : type.GetShortName())})".QuoteIfNeeded(),
           // var obj when obj.GetType()
           //   is var type && type.IsArray => $"{{ Object {{ Type: {type.ToLogString().Quote()} }} }}",
           _                             => $"{{ Object {{ Type: {value.GetType().ToLogString().QuoteIfNeeded()}, Value: {value.ToString().QuoteIfNeeded()} }} }}"
         };

    public static string Quote(this string str) => $"\"{str}\"";

    public static string QuoteIfNeeded(this string str)
    {
      foreach(var symbol in str)
        if(BadCharacters.Contains(symbol))
          return Quote(str);

      return str;
    }

    public static string ToLogString(this BuildResult buildResult) => buildResult.HasValue ? buildResult.Value.ToLogString() : "nothing";

    public static string ToLogString(this UnitId unitId)
      => $"{{ kind: {unitId.Kind.ToLogString()}, key: {unitId.Key.ToLogString()}}}";

    public static string ToLogString(this IBuildAction buildAction)
      => (buildAction switch
          {
            ILogable logable   => logable.ToLogString(),
            ILogable1 logable1 => logable1.ToLogString(),
            _                  => buildAction.GetType().GetShortName()
          });

    public static string ToLogString(this IEnumerable<UnitId> array)
    {
      var separator = string.Empty;

      StringBuilder sb = new("[");

      foreach(var unitId in array)
      {
        sb.Append(separator);
        sb.Append(unitId.ToLogString());
        separator = ", ";
      }

      sb.Append("]");
      return sb.ToString();
    }

    public static void WriteToLog(this WeightedBuildActionBag? actions, LogLevel logLevel)
    {
      Log.Execute(
        logLevel,
        () =>
        {
          if(actions is null)
            Log.WriteLine(logLevel, "null");
          else if(actions.Count == 1 && actions.Values.First().Count == 1) // a single action is found
          {
            var stage  = actions.Keys.First();
            var action = actions.Values.First()[0];
            WriteAction(stage, action);
          }
          else
            using(Log.IndentBlock(logLevel, "", "[]"))
            {
              Log.WriteLine(logLevel, "");
              foreach(var pair in actions)
              {
                var stage       = pair.Key;
                var actionsList = pair.Value;

                foreach(var action in actionsList)
                  WriteAction(stage, action);
              }}
        });

      void WriteAction(object stage, Weighted<IBuildAction> weightedAction)
        => Log.WriteLine(
          logLevel,
          "{{ Action: {0}, Stage: {1}, Weight: {2:n0} }}",
          weightedAction.Entity.ToLogString(),
          stage.ToString().QuoteIfNeeded(),
          weightedAction.Weight);
    }


    public static void WriteToLog(this Exception exc, Func<string> getTitle)
      => Log.Execute(
        LogLevel.Info,
        () =>
        {
          if(exc.Data.Contains(ExceptionConst.Logged)) return;
          exc.Data.Add(ExceptionConst.Logged, true);

          Log.WriteLine(LogLevel.Info, "");

          using(Log.NamedBlock(LogLevel.Info, getTitle))
          {
            var number    = 1;
            var exception = exc;

            while(exception is not null)
            {
              using(Log.NamedBlock(LogLevel.Info, $"Exception #{number}"))
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