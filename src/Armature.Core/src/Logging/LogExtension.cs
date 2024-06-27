using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using BeatyBit.Armature.Core.Sdk;
using JetBrains.Annotations;

namespace BeatyBit.Armature.Core;

[PublicAPI]
public static class LogExtension
{
  private static readonly HashSet<char> BadCharacters =
    ['$', '"', '{', '}', '[', ']', ':', '=', ',', '+', '#', '`', '^', '?', '!', '@', '*', '&', '/', '\\', ' ', '.'];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string GetFullName(this Type type)
    => type.IsGenericType
         ? string.Format(
           "{0}<{1}>",
           type.GetGenericTypeDefinition().FullName,
           string.Join(", ", type.GenericTypeArguments.Select(GetFullName).ToArray()))
         : type.FullName!;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string GetShortName(this Type type)
    => type.IsGenericType
         ? string.Format(
           "{0}<{1}>",
           type.GetGenericTypeDefinition().Name,
           string.Join(", ", type.GenericTypeArguments.Select(GetShortName).ToArray()))
         : type.Name;

  /// <summary>
  /// Returns the name of <paramref name="type" /> respecting <see cref="Log.LogFullTypeName" /> property.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToLogString(this Type type) => Log.LogFullTypeName ? type.GetFullName() : type.GetShortName();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ToLogString(this BuildResult buildResult) => buildResult.HasValue ? buildResult.Value.ToHoconString() : "nothing";

  /// <summary>
  /// Returns log representation of object, some objects logs in more friendly form then common <see cref="object.ToString" /> returns.
  /// </summary>
  public static string ToHoconString(this object? value)
  {
    try
    {
      return value switch
             {
               null                     => "null",
               string str               => str.QuoteIfNeeded(),
               BuildSession.Stack stack    => stack.ToHoconString(),
               ILogString logable       => logable.ToHoconString(),
               IBuildAction buildAction => buildAction.GetType().GetShortName().QuoteIfNeeded(),
               MethodBase methodInfo    => methodInfo.ToString().QuoteIfNeeded(),
               Type type                => $"typeof({(Log.LogFullTypeName ? type.GetFullName() : type.GetShortName())})".QuoteIfNeeded(),
               IEnumerable items        => $"[{string.Join(", ", items.Cast<object>().Select(_ => _.ToHoconString()))}]",
               bool b                   => b.ToString(CultureInfo.CurrentUICulture),
               char c                   => c.ToString(CultureInfo.CurrentUICulture),
               short s                  => s.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               ushort us                => us.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               int i                    => i.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               uint ui                  => ui.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               long l                   => l.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               ulong ul                 => ul.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               float f                  => f.ToString("n0", CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               double d                 => d.ToString(CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               decimal dc               => dc.ToString(CultureInfo.CurrentUICulture).QuoteIfNeeded(),
               _                        => $"{{ Object {{ Type: {value.GetType().ToLogString().QuoteIfNeeded()}, Value: {value.ToString().QuoteIfNeeded()} }} }}"
             };
    }
    catch(Exception exception)
    {
      return $"{nameof(ToHoconString)} {{ ExceptionType: {exception.GetType().ToLogString().QuoteIfNeeded()} }}";
    }
  }

  public static string ToHoconString(this Type              type)  => type.ToLogString().QuoteIfNeeded();
  public static string ToHoconArray(this  IEnumerable<Type> items) => $"[{string.Join(", ", items.Select(type => type.ToHoconString()))}]";

  public static string ToHoconString(this BuildSession.Stack stack)
  {
    var sb = new StringBuilder("[");

    var tillIndex = stack.Count - 1;

    for(var i = 0; i < tillIndex; i++)
    {
      sb.Append(stack[i]);
      sb.Append(", ");
    }

    sb.Append(stack[tillIndex]);
    sb.Append("]");
    return sb.ToString();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string Quote(this string str) => $"\"{str}\"";

  public static string QuoteIfNeeded(this string str)
  {
    var containSlash = false;
    foreach(var symbol in str)
    {
      if(symbol == '\\')
      {
        containSlash = true;
        break;
      }

      if(BadCharacters.Contains(symbol))
        return Quote(str);
    }

    return containSlash ? Quote(str.Replace(@"\", @"\\")) : str;
  }

  public static void WriteToLog(this WeightedBuildActionBag? actions, LogLevel logLevel, string propertyName)
  {
    Log.Execute(
      logLevel,
      () =>
      {
        Log.Write(logLevel, propertyName);

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
            foreach(var pair in actions)
            {
              var stage       = pair.Key;
              var actionsList = pair.Value;

              foreach(var action in actionsList)
                WriteAction(stage, action);
            }
          }
      });

    void WriteAction(object stage, Weighted<IBuildAction> weightedAction)
      => Log.WriteLine(
        logLevel,
        "{{ Action: {0}, Stage: {1}, Weight: {2} }}",
        weightedAction.Entity.ToHoconString(),
        stage.ToHoconString(),
        weightedAction.Weight.ToHoconString());
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string GetName(this IBuildAction buildAction)
    => $"{buildAction.GetType().GetShortName().QuoteIfNeeded()}";

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string ProcessMethod(this IBuildAction buildAction)
    => $"{GetName(buildAction)}.{nameof(IBuildAction.Process)}";

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string PostProcessMethod(this IBuildAction buildAction)
    => $"{GetName(buildAction)}.{nameof(IBuildAction.PostProcess)}";

  public static void WriteToLog(this Exception exception)
  {
    if(exception.Data.Contains(ExceptionConst.Logged)) return;

    Log.Execute(
      LogLevel.Info,
      () =>
      {
        exception.Data.Add(ExceptionConst.Logged, true);
        Log.WriteLine(LogLevel.Info, "Type: {0}", exception.GetType().ToHoconString());

        WriteHoconProperty(LogLevel.Info, "Message", exception.Message);

        WriteHoconProperty(LogLevel.Info, "StackTrace", exception.StackTrace);

        var data = exception.Data.Cast<DictionaryEntry>().Where(entry => !ExceptionConst.Logged.Equals(entry.Key)).ToArray();

        if(data.Length > 0)
          using(Log.NamedBlock(LogLevel.Info, "Data: "))
            foreach(var entry in data)
              Log.WriteLine(LogLevel.Info, "{0}: {1}", entry.Key.ToHoconString(), entry.Value.ToHoconString());

        if(exception is AggregateException aggregateException)
        {
          if(aggregateException.InnerExceptions.Count > 0)
            using(Log.IndentBlock(LogLevel.Info, "InnerExceptions: ", "[]"))
              foreach(var innerException in aggregateException.InnerExceptions)
                using(Log.NamedBlock(LogLevel.Info, ""))
                  innerException.WriteToLog();
        }
        else if(exception.InnerException is not null)
          using(Log.NamedBlock(LogLevel.Info, "InnerException: "))
            exception.InnerException.WriteToLog();
      });
  }

  private static void WriteHoconProperty(LogLevel logLevel, string propertyName, string text)
  {
    if(string.IsNullOrEmpty(text))
      Log.WriteLine(logLevel, "{0}: \"\"\"", propertyName);
    else
    {
      using var stringReader = new StringReader(text);
      Log.WriteLine(logLevel, "{0}: \"\"\"", propertyName);
      var line = stringReader.ReadLine();

      while(line is not null)
      {
        Log.WriteLine(logLevel, line);
        line = stringReader.ReadLine();
      }

      Log.WriteLine(logLevel, "\"\"\"");
    }
  }
}
