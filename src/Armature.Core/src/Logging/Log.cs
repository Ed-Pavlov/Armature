using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Armature.Core.Logging
{
  /// <summary>
  ///   Class is used to log Armature activities in human friendly form. Writes data into <see cref="System.Diagnostics.Trace" />, so
  ///   add a listener to see the log.
  /// </summary>
  public static class Log
  {
    private static readonly Stack<List<string>> DeferredContent = new();

    private static int      _indent;
    private static LogLevel _logLevel = LogLevel.None;

    /// <summary>
    ///   Set should full type name be logged or only short name w/o namespace to simplify reading.
    /// </summary>
    public static bool LogFullTypeName = false;

    /// <summary>
    ///   The count of spaces used to indent lines
    /// </summary>
    public static int IndentSize = 2;

    /// <summary>
    ///   Used to enable logging in a limited scope using "using" C# keyword
    /// </summary>
    public static IDisposable Enabled(LogLevel logLevel = LogLevel.Info)
    {
      var prevLevel = _logLevel;
      _logLevel = logLevel;
      return new Disposable(() => _logLevel = prevLevel);
    }

    public static void WriteLine(LogLevel logLevel, string line)
    {
      if(logLevel > _logLevel) return;

      DoWriteLine(GetIndent() + line);
    }

    [StringFormatMethod("format")]
    public static void WriteLine<T1>(LogLevel logLevel, string format, T1 p1) => WriteLine(logLevel, string.Format(format, p1));

    [StringFormatMethod("format")]
    public static void WriteLine<T1, T2>(LogLevel logLevel, string format, T1 p1, T2 p2) => WriteLine(logLevel, string.Format(format, p1, p2));

    [StringFormatMethod("format")]
    public static void WriteLine<T1, T2, T3>(LogLevel logLevel, string format, T1 p1, T2 p2, T3 p3)
      => WriteLine(logLevel, string.Format(format, p1, p2, p3));

    // [StringFormatMethod("format")]
    // public static void Write(LogLevel logLevel, string format, params object[] parameters)
    // {
    //   if(logLevel > _logLevel) return;
    //
    //   // ReSharper disable once CoVariantArrayConversion
    //   System.Diagnostics.Trace.Write(string.Format(format, parameters));
    // }


    [StringFormatMethod("format")]
    public static void WriteLine(LogLevel logLevel, string format, params object[] parameters) => WriteLine(logLevel, string.Format(format, parameters));

    /// <summary>
    ///   This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
    ///   use it calculating arguments for logging takes a time.
    /// </summary>
    [StringFormatMethod("format")]
    public static void WriteLine(LogLevel logLevel, [InstantHandle] Func<string> createMessage) => WriteLine(logLevel, createMessage());

    /// <summary>
    ///   This message calls <paramref name="createMessage"/> only if Logging is enabled for <paramref name="logLevel"/>,
    ///   use it calculating arguments for logging takes a time.
    /// </summary>
    [StringFormatMethod("format")]
    public static void WriteLine<T1>(LogLevel logLevel, [InstantHandle] Func<T1, string> createMessage, T1 v1) => WriteLine(logLevel, createMessage(v1));

    /// <summary>
    ///   Used to make an indented "block" in log data
    /// </summary>
    public static IDisposable Block(LogLevel logLevel) => logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;

    /// <summary>
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    [StringFormatMethod("format")]
    public static IDisposable Block(LogLevel logLevel, string format, params object[] parameters)
    {
      WriteLine(logLevel, format, parameters);

      return logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;
    }

    /// <summary>
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    public static IDisposable Block(LogLevel logLevel, Func<string> getName)
    {
      WriteLine(logLevel, getName);

      return logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;
    }

    /// <summary>
    ///   Used to make a named and indented "block" in log data
    /// </summary>
    public static IDisposable Block<T1>(LogLevel logLevel, Func<T1, string> getName, T1 v1)
    {
      WriteLine(logLevel, getName, v1);

      return logLevel <= _logLevel ? AddIndent(true) : DumbDisposable.Instance;
    }

    /// <summary>
    ///   Used to make an indented "block" in log data
    /// </summary>
    public static IDisposable AddIndent(bool newBlock = false, int count = 1) => new Indenter(newBlock, count);

    /// <summary>
    /// Executes action if <paramref name="logLevel"/> satisfies current Log level. See <see cref="Enabled"/> for details
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Execute(LogLevel logLevel, Action action)
    {
      if(logLevel > _logLevel) return;

      action();
    }


    /// <summary>
    ///   Returns the name of <paramref name="type" /> respecting <see cref="LogFullTypeName" /> property
    /// </summary>
    public static string ToLogString(this Type type) => LogFullTypeName ? type.GetFullName() : type.GetShortName();

    /// <summary>
    ///   Returns log representation of object, some objects logs in more friendly form then common <see cref="object.ToString" /> returns
    /// </summary>
    public static string ToLogString(this object? obj)
    {
      if(obj is null) return "null";

      return obj is Type type ? type.ToLogString() : obj.ToString();
    }

    public static IDisposable Deferred(LogLevel logLevel, Action<Action?> action)
    {
      if(_logLevel < logLevel) return DumbDisposable.Instance;

      DeferredContent.Push(new List<string>());
      var originalIndent = _indent;

      return new Disposable(
        () =>
        {
          var deferredContent = DeferredContent.Pop();

          action(
            deferredContent.Count == 0
              ? null
              : () =>
                {
                  var gap = new string(' ', _indent - originalIndent);

                  foreach(var line in deferredContent)
                    DoWriteLine(gap + line);
                });
        });
    }

    private static string GetIndent() => new(' ', _indent * IndentSize);

    private static void DoWriteLine(string line)
    {
      // all "Write" methods should at the end call this method, so we check for deferred in one place only
      if(DeferredContent.Count == 0)
        System.Diagnostics.Trace.WriteLine(line);
      else
        DeferredContent.Peek().Add(line);
    }

    private class Indenter : IDisposable
    {
      private readonly int  _indent;
      private readonly bool _isBlock;

      public Indenter(bool isBlock, int indent)
      {
        if(isBlock)
          WriteLine(LogLevel.Info, "{");

        _isBlock    =  isBlock;
        _indent     =  indent;
        Log._indent += _indent;
      }

      public void Dispose()
      {
        Log._indent -= _indent;

        if(_isBlock)
          WriteLine(LogLevel.Info, "}");
      }
    }

    private class Disposable : IDisposable
    {
      private readonly Action _action;

      public Disposable(Action action) => _action = action;

      public void Dispose() => _action();
    }

    private class DumbDisposable : IDisposable
    {
      public static readonly IDisposable Instance = new DumbDisposable();

      private DumbDisposable() { }

      public void Dispose()
      {
        // dumb
      }
    }
  }


  public enum LogLevel { None = 0, Info, Verbose, Trace }
}
